//+------------------------------------------------------------------+
//|                            Ehlers EnhancedSignalToNoiseRatio.mq5 |
//|                                Copyright 2020, Andrei Novichkov. |
//|                                               http://fxstill.com |
//|   Telegram: https://t.me/fxstill (Literature on cryptocurrencies,|
//|                                   development and code. )        |
//+------------------------------------------------------------------+

#property copyright "Copyright 2020, Andrei Novichkov."
#property link      "http://fxstill.com"
#property version   "1.00"
#property description "The Enhanced Signal To Noise Ratio:\nJohn Ehlers, \"Rocket Science for Traders.\", pg.87-88"


#property indicator_separate_window

#property indicator_level1     (double)6
#property indicator_levelstyle STYLE_SOLID

#property indicator_buffers 3

#property indicator_type1   DRAW_LINE 
#property indicator_width1  2
#property indicator_style1  STYLE_SOLID
#property indicator_color1  clrGreen 

#property indicator_label2  ""
#property indicator_type2   DRAW_NONE

#property indicator_label3  ""
#property indicator_type3   DRAW_NONE



double snr[];
double q3[], noise[];
static const int MINBAR = 5;

int OnInit()  {

   SetIndexBuffer(0,snr);
   SetIndexBuffer(1,q3);
   SetIndexBuffer(2,noise);
   
   IndicatorSetString(INDICATOR_SHORTNAME,"EnhancedSignalToNoiseRatio");
   IndicatorSetInteger(INDICATOR_DIGITS,_Digits);
   
   return INIT__SUCCEEDED();
}

double GetValue(const double& h[], const double& l[], int shift) {

   double smper = iCustom(NULL,0,"EhlersHilbertTransform",13,shift);
   double sm0   = iCustom(NULL,0,"EhlersHilbertTransform",2,shift);
   double sm2   = iCustom(NULL,0,"EhlersHilbertTransform",2,shift + 2);
 
   q3[shift] = 0.5 * (sm0 - sm2) * (0.1759 * smper + 0.4607);
   double i3 = 0.0;
   int sp = (int)MathCeil(smper / 2);
   if (sp == 0) sp = 1;
   
   for (int i = 0; i < sp; i++) {
       i3 += q3[shift + i];
   }    
   i3 = (1.57 * i3) / sp;
   
   double signal = MathPow(i3, 2) + MathPow(q3[shift], 2);
   
   noise[shift] = 0.1 * MathPow((h[shift] - l[shift]), 2) * 0.25 + 0.9 * noise[shift + 1];
   
   if (noise[shift] != 0.0 && signal != 0) {
      double s = ZerroIfEmpty(snr[shift + 1]);
      snr[shift] = 0.33 * 10 * MathLog(signal / noise[shift]) / MathLog(10) + 0.67 * s;
   } else {
      snr[shift] = 0;
   }
 
   return snr[shift];
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
                const long &tick_volume[],
                const long &volume[],
                const int &spread[]) {
      if(rates_total <= 4) return 0;
      int limit = rates_total - prev_calculated;
      if (limit == 0)        {   // Пришел новый тик 
      } else if (limit == 1) {   // Образовался новый бар
         GetValue(high, low, 1);
      } else if (limit > 1)  {   // Первый вызов индикатора, смена таймфрейма, подгрузка данных из истории
         ArrayInitialize(snr,   EMPTY_VALUE);
         ArrayInitialize(q3,    0);
         ArrayInitialize(noise, 0);
         limit = rates_total - MINBAR;
         for(int i = limit; i >= 1 && !IsStopped(); i--){
            GetValue(high, low, i);
         }//for(int i = limit + 1; i >= 0 && !IsStopped(); i--)
         return(rates_total);         
      }
      GetValue(high, low, 0);          
                
   return(rates_total);
}
