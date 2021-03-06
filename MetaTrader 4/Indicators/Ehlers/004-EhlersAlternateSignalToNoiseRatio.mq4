//+------------------------------------------------------------------+
//|                            EhlersAlternateSignalToNoiseRatio.mq5 |
//|                                Copyright 2020, Andrei Novichkov. |
//|  Main Site: http://fxstill.com                                   |
//|  Telegram:  https://t.me/fxstill (Literature on cryptocurrencies,| 
//|                                   development and code. )        |
//+------------------------------------------------------------------+
#property copyright "Copyright 2020, Andrei Novichkov."
#property link      "http://fxstill.com"
#property version   "1.00"
#property description "Telegram Channel: https://t.me/fxstill\n"
#property description "The Alternate Signal To Noise Ratio:\nJohn Ehlers, \"Rocket Science For Traders\", pg.84-85"

#property indicator_level1     (double)6
#property indicator_levelstyle STYLE_SOLID

#property indicator_separate_window

#property indicator_buffers 2
//--- plot snr
#property indicator_label1  "snr"
#property indicator_type1   DRAW_LINE
#property indicator_color1  clrGreen
#property indicator_style1  STYLE_SOLID
#property indicator_width1  2

#property indicator_label2  ""
#property indicator_type2   DRAW_NONE
//--- indicator buffers
double         snr[];     //Signal to Noise Ratio
double         range[];   //Noise

static const int MINBAR = 5;

//int h;
//+------------------------------------------------------------------+
//| Custom indicator initialization function                         |
//+------------------------------------------------------------------+
int OnInit()   {

   SetIndexBuffer(0,snr);
   SetIndexBuffer(1,range);
   
   IndicatorSetString(INDICATOR_SHORTNAME,"EhlersAlternateSignalToNoiseRatio");
   IndicatorSetInteger(INDICATOR_DIGITS,_Digits);     
//---
   return INIT__SUCCEEDED();
  }

  
  
void GetValue(const double& hg[], const double& lw[], int shift) {

   range[shift] = 0.1 * (hg[shift] - lw[shift]) + 0.9 * range[shift + 1];
   if (range[shift] <= 0) snr[shift] = 0;
   else {
      double s1 = ZerroIfEmpty(snr[shift + 1]);
      double re = iCustom(NULL,0,"EhlersHilbertTransform",10,shift);
      double im = iCustom(NULL,0,"EhlersHilbertTransform",11,shift);
      double s2 = re + im;
      if (s2 != 0.0) {
         s1 =  0.25 * (10 * MathLog((re + im)/ MathPow(range[shift], 2)) / MathLog(10) + 6) + 
                       0.75 * s1 ;
         snr[shift] = ZerroIfEmpty(s1);   
      }
   }
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
//| Custom indicator iteration function                              |
//+------------------------------------------------------------------+
int OnCalculate(const int rates_total,
                const int prev_calculated,
                const datetime &time[],
                const double &open[],
                const double &high[],
                const double &low[],
                const double &close[],
                const long &tick_volume[],
                const long &volume[],
                const int &spread[])
  {
      if(rates_total <= MINBAR) return 0;
      int limit = rates_total - prev_calculated;
      if (limit == 0)        {   // Пришел новый тик 
      } else if (limit == 1) {   // Образовался новый бар
      } else if (limit > 1)  {   // Первый вызов индикатора, смена таймфрейма, подгрузка данных из истории
         ArrayInitialize(snr,   EMPTY_VALUE);
         ArrayInitialize(range, EMPTY_VALUE);
         limit = rates_total - MINBAR;
         for(int i = limit; i >= 1 && !IsStopped(); i--){
            GetValue(high, low, i);
         }//for(int i = limit + 1; i >= 0 && !IsStopped(); i--)
         return(rates_total);         
      }
      GetValue(high, low, 0);          
   return(rates_total);
}
