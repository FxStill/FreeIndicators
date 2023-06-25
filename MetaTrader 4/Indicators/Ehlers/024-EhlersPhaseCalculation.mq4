//+------------------------------------------------------------------+
//|                                       EhlersPhaseCalculation.mq5 |
//|                                Copyright 2020, Andrei Novichkov. |
//|  Main Site: http://fxstill.com                                   |
//|  Telegram:  https://t.me/fxstill (Literature on cryptocurrencies,| 
//|                                   development and code. )        |
//+------------------------------------------------------------------+
#property copyright "Copyright 2020, Andrei Novichkov."
#property link      "http://fxstill.com"
#property version   "1.00"
#property description "Telegram Channel: https://t.me/fxstill\n"
#property description "The Phase Calculation:\nJohn Ehlers, \"Stocks and Commodities Magazine 11/1996\""


#define NAME (string)"EhlersPhaseCalculation"

#property indicator_separate_window

#property indicator_buffers 2
//--- plot snr
#property indicator_label1  "Phase"
#property indicator_type1   DRAW_LINE
#property indicator_color1  clrRed
#property indicator_style1  STYLE_SOLID
#property indicator_width1  2

#property indicator_label2  "Signal"
#property indicator_type2   DRAW_LINE
#property indicator_color2  clrDodgerBlue
#property indicator_style2  STYLE_SOLID
#property indicator_width2  2


//--- indicator buffers
double         phase[];
double         signal[];

input int length = 15; //Length

static const int MINBAR = length + 1;

//+------------------------------------------------------------------+
//| Custom indicator initialization function                         |
//+------------------------------------------------------------------+
int OnInit()   {
//--- indicator buffers mapping
   SetIndexBuffer(0,phase);
   SetIndexBuffer(1,signal);
   
   IndicatorSetString(INDICATOR_SHORTNAME, NAME);
   IndicatorSetInteger(INDICATOR_DIGITS,_Digits);     
//---
   return INIT__SUCCEEDED();
  }

 
  
void GetValue(const double& h[], const double& l[], int shift) {

   double weight = 0.0, radians = 0.0, realP = 0.0, imagP = 0.0;
   
   for (int i = 0; i < length; i++) {
      weight = (h[shift + i] + l[shift + i]) / 2;
      radians = 2 * M_PI * i / length;
      realP = realP + MathCos(radians) * weight;
      imagP = imagP + MathSin(radians) * weight;
   } 
   phase[shift] = (MathAbs(realP) > 0.001)? MathArctan(imagP / realP) : M_PI_2 * sign(imagP);
   if (realP < 0) 
       phase[shift] = phase[shift] + M_PI_2;
   if (phase[shift] < 0) 
       phase[shift] = phase[shift] + M_PI_2;
   if (phase[shift] > M_PI * 2) 
       phase[shift] = phase[shift] - M_PI_2;
   phase[shift] = 180 * phase[shift] / M_PI;
   signal[shift] = SimpleMA(shift, length / 2, phase);

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
int sign(double x) {
   if (x == 0.0) return 0;
   if (x >  0.0) return 1;
   return -1;
}
double SimpleMA(const int position,const int period,const double &price[])  {

   double result = 0.0;
   for(int i = 0; i < period; i++) 
      result += price[position + i];
   result /= period;
   return(result);
}// double SimpleMA(const int position,const int period,const double &price[])
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
                const long& tick_volume[],
                const long& volume[],
                const int& spread[])
  {
      if(rates_total < MINBAR) return 0;
      int limit = rates_total - prev_calculated;
      if (limit == 0)        {   // Пришел новый тик 
      } else if (limit == 1) {   // Образовался новый бар
         GetValue(high, low, 1);
         return(rates_total); 
      } else if (limit > 1)  {   // Первый вызов индикатора, смена таймфрейма, подгрузка данных из истории
         ArrayInitialize(phase, EMPTY_VALUE);
         ArrayInitialize(signal, EMPTY_VALUE);
         limit = rates_total - MINBAR;
         for(int i = limit; i >= 1 && !IsStopped(); i--){
            GetValue(high, low, i);
         }//for(int i = limit + 1; i >= 0 && !IsStopped(); i--)
         return(rates_total);         
      }
      GetValue(high, low, 0);          
   return(rates_total);
}
