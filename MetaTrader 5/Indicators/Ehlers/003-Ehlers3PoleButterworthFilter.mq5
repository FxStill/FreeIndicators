//+------------------------------------------------------------------+
//|                                 Ehlers3PoleButterworthFilter.mq5 |
//|                                Copyright 2020, Andrei Novichkov. |
//|                                               http://fxstill.com |
//|   Telegram: https://t.me/fxstill (Literature on cryptocurrencies,| 
//|                                   development and code. )        |
//+------------------------------------------------------------------+
#property copyright "Copyright 2020, Andrei Novichkov."
#property link      "http://fxstill.com"
#property version   "1.00"

#property description "The 3 Pole Butterworth Filter:\nJohn Ehlers, \"Cybernetic Analysis For Stocks And Futures\", pg.192"

#property indicator_chart_window

#property indicator_applied_price PRICE_MEDIAN

#property indicator_buffers 2
#property indicator_plots   1

#property indicator_type1   DRAW_COLOR_LINE 
#property indicator_width1  2
#property indicator_style1  STYLE_SOLID
#property indicator_color1  clrGreen, clrRed, clrLimeGreen

#define M_SQRT3 sqrt(3)

input int length = 15; 

double bf[];
double cf[]; 

double a, b, c, cf1, cf2, cf3, cf4;

static const int MINBAR = 5;

int OnInit() {

   SetIndexBuffer(0,bf,INDICATOR_DATA);
   SetIndexBuffer(1,cf,INDICATOR_COLOR_INDEX); 
   ArraySetAsSeries(bf,true);
   ArraySetAsSeries(cf,true); 
   
   IndicatorSetString(INDICATOR_SHORTNAME,"Ehlers3PoleButterworthFilter");
   IndicatorSetInteger(INDICATOR_DIGITS,_Digits);
   
   
   a   = MathExp(-M_PI / length);
   b   = 2 * a * MathCos(M_SQRT3 * 180 / length);
   c   = MathPow(a, 2);
   cf2 = b + c;
   cf3 = -(c + b * c);
   cf4 = MathPow(c, 2);
   cf1 = (1 - b + c) * (1 - c) / 8;   

   return(INIT_SUCCEEDED);
}

void GetValue(const double& price[], int shift) {

   double b1 = ZerroIfEmpty(bf[shift + 1]);
   double b2 = ZerroIfEmpty(bf[shift + 2]);
   double b3 = ZerroIfEmpty(bf[shift + 3]);
   
   bf[shift] = cf1 *(price[shift] + 3 * price[shift + 1] + 3 * price[shift + 2] + price[shift + 3]) + 
               cf2 * b1 + cf3 * b2 + cf4 * b3;
               
   if (bf[shift] < price[shift]) cf[shift] = 2 ; 
   else
      if (bf[shift] > price[shift]) cf[shift] = 1 ;
               
}

int OnCalculate(const int rates_total,
                const int prev_calculated,
                const int begin,
                const double &price[])  {
      if(rates_total <= MINBAR) return 0;
      ArraySetAsSeries(price,true);    
      int limit = rates_total - prev_calculated;
      if (limit == 0)        {   // Пришел новый тик 
      } else if (limit == 1) {   // Образовался новый бар
         GetValue(price, 1);  
         return(rates_total);   
      } else if (limit > 1)  {   // Первый вызов индикатора, смена таймфрейма, подгрузка данных из истории
         ArrayInitialize(bf,EMPTY_VALUE);
         ArrayInitialize(cf,0);
         limit = rates_total - MINBAR;
         for(int i = limit; i >= 1 && !IsStopped(); i--){
            GetValue(price, i);
         }//for(int i = limit + 1; i >= 0 && !IsStopped(); i--)
         return(rates_total);         
      }
      GetValue(price, 0);          
      return(rates_total);
}
