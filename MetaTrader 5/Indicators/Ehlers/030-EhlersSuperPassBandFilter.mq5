//+------------------------------------------------------------------+
//|                                    EhlersSuperPassBandFilter.mq5 |
//|                                Copyright 2020, Andrei Novichkov. |
//|  Main Site: http://fxstill.com                                   |
//|  Telegram:  https://t.me/fxstill (Literature on cryptocurrencies,| 
//|                                   development and code. )        |
//+------------------------------------------------------------------+
#property copyright "Copyright 2020, Andrei Novichkov."
#property link      "http://fxstill.com"
#property version   "1.00"
#property description "Telegram Channel: https://t.me/fxstill\n"
#property description "The Super PassBand Filter:\nJohn Ehlers, \"Stocks & Commodities V. 34:07\", pg.10-13"

#property indicator_separate_window
#property indicator_applied_price PRICE_CLOSE


#property indicator_buffers 4
#property indicator_plots 3

#property indicator_type1   DRAW_COLOR_LINE 
#property indicator_width1  2
#property indicator_style1  STYLE_SOLID
#property indicator_color1  clrGreen, clrRed, clrLimeGreen 

#property indicator_type2   DRAW_LINE 
#property indicator_width2  2
#property indicator_style2  STYLE_SOLID
#property indicator_color2  clrOrange 

#property indicator_type3   DRAW_LINE 
#property indicator_width3  2
#property indicator_style3  STYLE_SOLID
#property indicator_color3  clrOrange 

input int eshort = 40;//EMA Short Period
input int elong  = 60;//EMA Long Period
input int rms    = 50;//RMS Period

double rms1[], rms2[], spbf[];
double cf[];

static const int MINBAR = rms;

double a1, a2;

int OnInit()  {

   SetIndexBuffer(0,spbf,INDICATOR_DATA);
   SetIndexBuffer(1,cf,INDICATOR_COLOR_INDEX); 
      
   SetIndexBuffer(2,rms1,INDICATOR_DATA);
   SetIndexBuffer(3,rms2,INDICATOR_DATA);
   

   
   ArraySetAsSeries(spbf,true);
   ArraySetAsSeries(rms1,true); 
   ArraySetAsSeries(rms2,true); 
   ArraySetAsSeries(cf,  true); 
   
   
   IndicatorSetString(INDICATOR_SHORTNAME,"EhlersSuperPassBandFilter");
   IndicatorSetInteger(INDICATOR_DIGITS,_Digits);
   
   a1 = 5.0 / eshort;
   a2 = 5.0 / elong;
   

   return INIT__SUCCEEDED();
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

void GetValue(const double& price[], int shift) {

   double s1 = ZerroIfEmpty(spbf[shift + 1]);// == EMPTY_VALUE)? 0: spbf[shift + 1];
   double s2 = ZerroIfEmpty(spbf[shift + 2]);// == EMPTY_VALUE)? 0: spbf[shift + 2];
   spbf[shift] = (a1 - a2) * price[shift] + 
               ( a2 * (1 - a1) - a1 * (1 - a2) ) * price[shift + 1] + 
               (2 - a1  - a2) * s1 - 
               (1 - a1) * (1 - a2) * s2;
   double t = 0;
   for (int i = 0; i < rms; ++i) {
      if (spbf[shift + i] == EMPTY_VALUE) continue;
      t += pow(spbf[shift + i], 2);
   }
   rms1[shift] = MathSqrt(t / rms);
   rms2[shift] = -rms1[shift];      
   
   if (spbf[shift] < 0) cf[shift] = 1 ; 
   else
      if (spbf[shift] > 0) cf[shift] = 2 ;
             
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
      } else if (limit > 1)  {   // Первый вызов индикатора, смена таймфрейма, подгрузка данных из истории
         ArrayInitialize(spbf, EMPTY_VALUE);
         ArrayInitialize(rms1, EMPTY_VALUE);
         ArrayInitialize(rms2, EMPTY_VALUE);
         ArrayInitialize(cf,   0);
         limit = rates_total - MINBAR;
         for(int i = limit; i >= 1 && !IsStopped(); i--){
            GetValue(price, i);
         }//for(int i = limit + 1; i >= 0 && !IsStopped(); i--)
         return(rates_total);         
      }
      GetValue(price, 0);          
                
   return(rates_total);
}
