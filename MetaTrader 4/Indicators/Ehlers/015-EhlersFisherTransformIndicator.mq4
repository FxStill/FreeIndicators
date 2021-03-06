//+------------------------------------------------------------------+
//|                               EhlersFisherTransformIndicator.mq5 |
//|                                Copyright 2020, Andrei Novichkov. |
//|                                               http://fxstill.com |
//|   Telegram: https://t.me/fxstill (Literature on cryptocurrencies,| 
//|                                   development and code. )        |
//+------------------------------------------------------------------+
#property copyright "Copyright 2020, Andrei Novichkov."
#property link      "http://fxstill.com"
#property version   "1.00"

#property description "The Fisher Transform Indicator:\nJohn Ehlers, \"Cybernetic Analysis For Stocks And Futures\", pg.7-8"


#property indicator_separate_window

#property indicator_buffers 3
//--- plot v3
#property indicator_label1  "fb"
#property indicator_type1   DRAW_LINE
#property indicator_color1  clrGreen
#property indicator_style1  STYLE_SOLID
#property indicator_width1  1
//--- plot trigger
#property indicator_label2  "f1"
#property indicator_type2   DRAW_LINE
#property indicator_color2  clrDarkBlue
#property indicator_style2  STYLE_SOLID
#property indicator_width2  1

#property indicator_label3  ""
#property indicator_type3   DRAW_NONE
//--- input parameters
input int      length=10;
//--- indicator buffers
double         fb[];
double         f1[];
double         v1[];


static const int MINBAR = 5;
//+------------------------------------------------------------------+
//| Custom indicator initialization function                         |
//+------------------------------------------------------------------+
int OnInit()
  {
//--- indicator buffers mapping
   SetIndexBuffer(0,fb);
   SetIndexBuffer(1,f1);
   SetIndexBuffer(2,v1);
   
//---
   IndicatorSetString(INDICATOR_SHORTNAME,"EhlersFisherTransformIndicator");
   IndicatorSetInteger(INDICATOR_DIGITS,_Digits);
   
   return INIT__SUCCEEDED();
  }
  
void GetValue(const double& h[], const double& l[], int shift) {
   
   double  p0 = (h[shift] + l[shift]) / 2;
   double mx = 0.0;
   double mn = 1000000.0;
   for (int i = shift; i < shift + length; i++) {
      double t = (h[i] + l[i]) / 2;
      if (t > mx) mx = t;
      if (t < mn) mn = t;
   }
   if (mx != mn)
      v1[shift] = (p0 - mn)/(mx - mn) - 0.5 + 0.5 * v1[shift + 1];
   else v1[shift] = 0;   
   v1[shift] = MathMax(MathMin(v1[shift], 0.999), -0.999);   

   double ft = ZerroIfEmpty(fb[shift + 1]);
   fb[shift] = 0.25 * MathLog((1 + v1[shift])/(1 - v1[shift])) + 0.5 * ft;   

   f1[shift] = fb[shift + 1];
   
}    
int INIT__SUCCEEDED() {
   PlaySound("ok.wav");
   string cm = "Subscribe! https://t.me/fxstill";
   Print(cm);
   Comment("\n"+cm);
   return INIT_SUCCEEDED;
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
                const long& tick_volume[],
                const long& volume[],
                const int& spread[])
  {
      if(rates_total <= MINBAR) return 0;
      int limit = rates_total - prev_calculated;
      if (limit == 0)        {   // Пришел новый тик 
      } else if (limit == 1) {   // Образовался новый бар
         GetValue(high, low, 1); 
      } else if (limit > 1)  {   // Первый вызов индикатора, смена таймфрейма, подгрузка данных из истории
         ArrayInitialize(fb,EMPTY_VALUE);
         ArrayInitialize(f1,EMPTY_VALUE);
         ArrayInitialize(v1,0);
         limit = rates_total - MINBAR;
         for(int i = limit; i >= 1 && !IsStopped(); i--){
            GetValue(high, low, i);
         }//for(int i = limit + 1; i >= 0 && !IsStopped(); i--)
         return(rates_total);         
      }
      GetValue(high, low, 0); 

   return(rates_total);
  }
  
double ZerroIfEmpty(double value) {
   if (value >= EMPTY_VALUE || value <= -EMPTY_VALUE) return 0.0;
   return value;
}  
  
void OnDeinit(const int reason) {
  Comment("");
}
//+------------------------------------------------------------------+
