//+------------------------------------------------------------------+
//|                               Ehlers2PoleSuperSmootherFilter.mq5 |
//|                                Copyright 2020, Andrei Novichkov. |
//|  Main Site: http://fxstill.com                                   |
//|  Telegram:  https://t.me/fxstill (Literature on cryptocurrencies,| 
//|                                   development and code. )        |
//+------------------------------------------------------------------+
#property copyright "Copyright 2020, Andrei Novichkov."
#property link      "http://fxstill.com"
#property version   "1.00"
#property description "Telegram Channel: https://t.me/fxstill\n"
#property description "The 2 Pole Super Smoother Filter:\nJohn Ehlers, \"Cybernetic Analysis For Stocks And Futures\", pg.202"


#property indicator_chart_window
#property indicator_applied_price PRICE_MEDIAN

#property indicator_buffers 2
#property indicator_plots   1
//--- plot ssf
#property indicator_label1  "ssf"
#property indicator_type1   DRAW_COLOR_LINE
#property indicator_color1  clrGreen,clrRed,clrLimeGreen
#property indicator_style1  STYLE_SOLID
#property indicator_width1  2

input int length = 15; //Length
//--- indicator buffers
double         sb[];
double         sc[];

static const int MINBAR = 3;
//+------------------------------------------------------------------+
//| Custom indicator initialization function                         |
//+------------------------------------------------------------------+
double c1, c2, c3;
int OnInit() {
//--- indicator buffers mapping
   SetIndexBuffer(0,sb,INDICATOR_DATA);
   SetIndexBuffer(1,sc,INDICATOR_COLOR_INDEX);
   ArraySetAsSeries(sb,true);
   ArraySetAsSeries(sc,true); 
   
   IndicatorSetString(INDICATOR_SHORTNAME,"Ehlers2PoleSuperSmootherFilter");
   IndicatorSetInteger(INDICATOR_DIGITS,_Digits);   

   double a1 = MathExp(-M_SQRT2 * M_PI / length);
   double b1 = 2 * a1 * MathCos(M_SQRT2 * M_PI / length);
   c2 = b1;
   c3 = -MathPow(a1, 2);
   c1 = 1 - c2 - c3;

   return INIT__SUCCEEDED();
}

void GetValue(const double& price[], int shift) {

   double s1 = ZerroIfEmpty(sb[shift + 1]);
   double s2 = ZerroIfEmpty(sb[shift + 2]);

   sb[shift] = c1 * price[shift] + c2 * s1 + c3 * s2;

   if (sb[shift] < price[shift]) sc[shift] = 2 ; 
   else
      if (sb[shift] > price[shift]) sc[shift] = 1 ;   

}
//+------------------------------------------------------------------+
//| Custom indicator iteration function                              |
//+------------------------------------------------------------------+
int OnCalculate(const int rates_total,
                const int prev_calculated,
                const int begin,
                const double &price[]) {
      if(rates_total <= MINBAR) return 0;
      ArraySetAsSeries(price,true);    
      int limit = rates_total - prev_calculated;
      if (limit == 0)        {   // Пришел новый тик 
      } else if (limit == 1) {   // Образовался новый бар
         GetValue(price, 1);  
         return(rates_total);   
      } else if (limit > 1)  {   // Первый вызов индикатора, смена таймфрейма, подгрузка данных из истории
         ArrayInitialize(sb,   EMPTY_VALUE);
         ArrayInitialize(sc,  0);
         limit = rates_total - MINBAR;
         for(int i = limit; i >= 1 && !IsStopped(); i--){
            GetValue(price, i);
         }//for(int i = limit + 1; i >= 0 && !IsStopped(); i--)
         return(rates_total);         
      }
      GetValue(price, 0);          
                
   return(rates_total);
}
int INIT__SUCCEEDED() {
   PlaySound("ok.wav");
   string cm = "Subscribe! https://t.me/fxstill";
   Print(cm);
   Comment("\n"+cm);
   return INIT_SUCCEEDED;
}
double ZerroIfEmpty(double value) {
   if (value >= EMPTY_VALUE || value <= -EMPTY_VALUE) return 0.0;
   return value;
}  
void OnDeinit(const int reason) {
  Comment("");
}
//+------------------------------------------------------------------+
