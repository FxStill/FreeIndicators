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


#property indicator_buffers 3

#property indicator_type1   DRAW_LINE 
#property indicator_width1  2
#property indicator_style1  STYLE_SOLID
#property indicator_color1  clrLimeGreen 

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

static const int MINBAR = rms;

double a1, a2;

int OnInit()  {

   SetIndexBuffer(0,spbf);
   SetIndexBuffer(1,rms1);
   SetIndexBuffer(2,rms2);

   IndicatorSetString(INDICATOR_SHORTNAME,"EhlersSuperPassBandFilter");
   IndicatorSetInteger(INDICATOR_DIGITS,_Digits);
   
   a1 = 5.0 / eshort;
   a2 = 5.0 / elong;
   

   return INIT__SUCCEEDED();
}

void GetValue(const double& h[], const double& l[], int shift) {

   double price0 = (h[shift] + l[shift]) / 2;
   double price1 = (h[shift + 1] + l[shift + 1]) / 2;
   double s1 = ZerroIfEmpty(spbf[shift + 1]);
   double s2 = ZerroIfEmpty(spbf[shift + 2]);
   
   spbf[shift] = (a1 - a2) * price0 + 
               ( a2 * (1 - a1) - a1 * (1 - a2) ) * price1 + 
               (2 - a1  - a2) * s1 - 
               (1 - a1) * (1 - a2) * s2;
   double t = 0;
   for (int i = 0; i < rms; ++i) {
      if (spbf[shift + i] == EMPTY_VALUE) continue;
      t += pow(spbf[shift + i], 2);
   }
   rms1[shift] = MathSqrt(t / rms);
   rms2[shift] = -rms1[shift];      
   
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

int OnCalculate(const int rates_total,
                const int prev_calculated,
                const datetime &time[],
                const double &open[],
                const double &high[],
                const double &low[],
                const double &close[],
                const long& tick_volume[],
                const long& volume[],
                const int& spread[])
  {
      if(rates_total <= MINBAR) return 0;
      int limit = rates_total - prev_calculated;
      if (limit == 0)        {   // Пришел новый тик 
      } else if (limit == 1) {   // Образовался новый бар
         GetValue(high, low, 1);          
         return(rates_total);      
      } else if (limit > 1)  {   // Первый вызов индикатора, смена таймфрейма, подгрузка данных из истории
         ArrayInitialize(spbf, EMPTY_VALUE);
         ArrayInitialize(rms1, EMPTY_VALUE);
         ArrayInitialize(rms2, EMPTY_VALUE);
         limit = rates_total - MINBAR;
         for(int i = limit; i >= 1 && !IsStopped(); i--){
            GetValue(high, low, i);
         }//for(int i = limit + 1; i >= 0 && !IsStopped(); i--)
         return(rates_total);         
      }
      GetValue(high, low, 0);          
                
   return(rates_total);
}
