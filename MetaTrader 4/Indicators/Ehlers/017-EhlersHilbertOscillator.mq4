//+------------------------------------------------------------------+
//|                            EhlersHilbertOscillator.mq5 |
//|                                Copyright 2020, Andrei Novichkov. |
//|  Main Site: http://fxstill.com                                   |
//|  Telegram:  https://t.me/fxstill (Literature on cryptocurrencies,| 
//|                                   development and code. )        |
//+------------------------------------------------------------------+
#property copyright "Copyright 2020, Andrei Novichkov."
#property link      "http://fxstill.com"
#property version   "1.00"
#property description "Telegram Channel: https://t.me/fxstill\n"
#property description "The Hilbert Oscillator:\nJohn Ehlers, \"Rocket Science For Traders\", pg.90-91"

#property indicator_separate_window

#property indicator_buffers 4

#property indicator_label1  "V3"
#property indicator_type1   DRAW_LINE
#property indicator_color1  clrDodgerBlue
#property indicator_style1  STYLE_SOLID
#property indicator_width1  2

#property indicator_label2  "I3"
#property indicator_type2   DRAW_LINE
#property indicator_color2  clrRed
#property indicator_style2  STYLE_SOLID
#property indicator_width2  2

#property indicator_label3  ""
#property indicator_type3   DRAW_NONE

double         v1[];
double         i3[];
double         q3[];

static const int MINBAR = 5;


int OnInit()   {
   SetIndexBuffer(0,v1);
   SetIndexBuffer(1,i3);
   SetIndexBuffer(2,q3);
   
   IndicatorSetString(INDICATOR_SHORTNAME,"EhlersHilbertOscillator");
   IndicatorSetInteger(INDICATOR_DIGITS,_Digits);     

   return INIT__SUCCEEDED();
  }

  
void GetValue(const double& h[], const double& l[], int shift) {

   double SmoothPeriod = iCustom(NULL,0,"EhlersHilbertTransform",13,shift);
   int sp2 = (int)MathCeil(SmoothPeriod / 2);
   if (shift + sp2 >= ArraySize(q3) || sp2 == 0 ) return;
   if (sp2 < 3) sp2 = 3;
   
   double Smooth0 = iCustom(NULL,0,"EhlersHilbertTransform",2,shift);
   double Smooth2 = iCustom(NULL,0,"EhlersHilbertTransform",2,shift+2);
   q3[shift] = 0.5 * (Smooth0 - Smooth2) * (0.1759 * SmoothPeriod + 0.4607);
   i3[shift] = q3[shift];
   i3[shift] = 0.0;
   for (int i = 0; i < sp2; i++) 
      i3[shift] = i3[shift] + q3[shift + i];
   i3[shift] = (1.57 * i3[shift]) / sp2;
   
   double sp4 = MathCeil(SmoothPeriod / 4);
   v1[shift] = 0.0;
   for (i = 0; i < sp4; i++)
      v1[shift] = v1[shift] + q3[shift + i];
   v1[shift] = 1.25 * v1[shift] / sp4;

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
      if(rates_total < MINBAR) return 0;
      int limit = rates_total - prev_calculated;
      if (limit == 0)        {   // Пришел новый тик 
      } else if (limit == 1) {   // Образовался новый бар
         GetValue(high, low, 1);
      } else if (limit > 1)  {   // Первый вызов индикатора, смена таймфрейма, подгрузка данных из истории
         ArrayInitialize(v1,   EMPTY_VALUE);
         ArrayInitialize(i3,   EMPTY_VALUE);
         ArrayInitialize(q3, 0);
         limit = rates_total - MINBAR;
         for(int i = limit; i >= 1 && !IsStopped(); i--){
            GetValue(high, low, i);
         }//for(int i = limit + 1; i >= 0 && !IsStopped(); i--)
         return(rates_total);         
      }
      GetValue(high, low, 0);          
   return(rates_total);
}
