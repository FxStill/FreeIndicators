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
#property indicator_plots   1
//--- plot snr
#property indicator_label1  "snr"
#property indicator_type1   DRAW_LINE
#property indicator_color1  clrGreen
#property indicator_style1  STYLE_SOLID
#property indicator_width1  2
//--- indicator buffers
double         snr[];     //Signal to Noise Ratio
double         range[];   //Noise

static const int MINBAR = 5;

int h;
//+------------------------------------------------------------------+
//| Custom indicator initialization function                         |
//+------------------------------------------------------------------+
int OnInit()   {
   h = iCustom(NULL,0,"EhlersHilbertTransform");
   if (h == INVALID_HANDLE) {
      Print("Error while creating \"EhlersHilbertTransform\"");
      return (INIT_FAILED);
   }
//--- indicator buffers mapping
   SetIndexBuffer(0,snr,INDICATOR_DATA);
   SetIndexBuffer(1,range,INDICATOR_CALCULATIONS);
   ArraySetAsSeries(snr,true);
   ArraySetAsSeries(range,true); 
   
   IndicatorSetString(INDICATOR_SHORTNAME,"EhlersAlternateSignalToNoiseRatio");
   IndicatorSetInteger(INDICATOR_DIGITS,_Digits);     
//---
   return INIT__SUCCEEDED();
  }

  
  
void GetValue(const double& hg[], const double& lw[], int shift) {
   double re[1], im[1], smooth[1];
   if (CopyBuffer(h, 10, shift, 1, re) <= 0) return;
   if (CopyBuffer(h, 11, shift, 1, im) <= 0) return;
   if (CopyBuffer(h,  2, shift, 1, smooth) <= 0) return;
   range[shift] = 0.1 * (hg[shift] - lw[shift]) + 0.9 * range[shift + 1];
   if (range[shift] <= 0) snr[shift] = 0;
   else {
      double s1 = ZerroIfEmpty(snr[shift + 1]);
      double s2 = re[0] + im[0];
      if (s2 != 0.0) {
         s1 =  0.25 * (10 * MathLog((re[0] + im[0])/ MathPow(range[shift], 2)) / MathLog(10) + 6) + 
                       0.75 * s1 ;
         snr[shift] = ZerroIfEmpty(s1);   
      }
   }
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
      ArraySetAsSeries(high,true);    
      ArraySetAsSeries(low, true);
      int limit = rates_total - prev_calculated;
      if (limit == 0)        {   // Пришел новый тик 
      } else if (limit == 1) {   // Образовался новый бар
         GetValue(high, low, 1);  
         return(rates_total);   
      } else if (limit > 1)  {   // Первый вызов индикатора, смена таймфрейма, подгрузка данных из истории
         ArrayInitialize(snr,   EMPTY_VALUE);
         ArrayInitialize(range, 0);
         limit = rates_total - MINBAR;
         for(int i = limit; i >= 1 && !IsStopped(); i--){
            GetValue(high, low, i);
         }//for(int i = limit + 1; i >= 0 && !IsStopped(); i--)
         return(rates_total);         
      }
      GetValue(high, low, 0);          
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
   if (h != INVALID_HANDLE)
      IndicatorRelease(h);
}  
//+------------------------------------------------------------------+
