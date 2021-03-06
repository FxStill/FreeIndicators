//+------------------------------------------------------------------+
//|                        EhlersFisherStochasticCenterOfGravity.mq5 |
//|                                Copyright 2020, Andrei Novichkov. |
//|  Main Site: http://fxstill.com                                   |
//|  Telegram:  https://t.me/fxstill (Literature on cryptocurrencies,| 
//|                                   development and code. )        |
//+------------------------------------------------------------------+
#property copyright "Copyright 2020, Andrei Novichkov."
#property link      "http://fxstill.com"
#property version   "1.00"
#property description "Telegram Channel: https://t.me/fxstill\n"
#property description "The Fisher Stochastic CG (Center Of Gravity):\nJohn Ehlers"

#property indicator_separate_window

#property indicator_buffers 5
//--- plot v3
#property indicator_label1  "CG"
#property indicator_type1   DRAW_LINE
#property indicator_color1  clrLimeGreen
#property indicator_style1  STYLE_SOLID
#property indicator_width1  1
//--- plot trigger
#property indicator_label2  "trigger"
#property indicator_type2   DRAW_LINE
#property indicator_color2  clrDarkBlue
#property indicator_style2  STYLE_SOLID
#property indicator_width2  1

#property indicator_label3  ""
#property indicator_type3   DRAW_NONE

#property indicator_label4  ""
#property indicator_type4   DRAW_NONE

#property indicator_label5  ""
#property indicator_type5   DRAW_NONE



//--- input parameters
input int      length = 8;
//--- indicator buffers
double         v3[];
double         trigger[];
double         v1[];
double         v2[];
double         sg[];

static const int MINBAR = length + 1;
double lw;
//+------------------------------------------------------------------+
//| Custom indicator initialization function                         |
//+------------------------------------------------------------------+
int OnInit()
  {
//--- indicator buffers mapping
   SetIndexBuffer(0,v3);
   SetIndexBuffer(1,trigger);
   SetIndexBuffer(2,v1);
   SetIndexBuffer(3,v2);
   SetIndexBuffer(4,sg);
//---
   IndicatorSetString(INDICATOR_SHORTNAME,"EhlersFisherStochasticCenterOfGravity");
   IndicatorSetInteger(INDICATOR_DIGITS,_Digits);
   
   lw = (length + 1) / 2;
   
   return INIT__SUCCEEDED();
  }
  
void GetValue(const double& h[], const double& l[], int shift) {
   
   double num = 0.0;
   double denom = 0.0;
   double p = 0.0;
   for (int i = 0; i < length; i++) {
      p = (h[shift + i] + l[shift + i]) / 2;
      num = num + (1 + i) * p;
      denom = denom + p;   
   } 
   if (denom != 0)
      sg[shift] =  lw - num / denom;
   else sg[shift] = 0;   
//   v3[shift] = num / denom;
   
   int maxCg = ArrayMaximum(sg, length, shift);
   int minCg = ArrayMinimum(sg, length, shift);
   if (maxCg == -1 || minCg == -1) return;
   if (sg[maxCg] != sg[minCg])
      v1[shift] = (sg[shift] - sg[minCg]) / (sg[maxCg] - sg[minCg]);
   else v1[shift] = 0;   
   v2[shift] = (4 * v1[shift] + 3 * v1[shift + 1] + 2 * v1[shift + 2] + v1[shift + 3]) / 10;     
   v3[shift] = 0.5 * MathLog((1 + (1.98 * (v2[shift] - 0.5))) / (1 - (1.98 * (v2[shift] - 0.5))));

   trigger[shift] = v3[shift + 1];    
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
                const long& tick_volume[],
                const long& volume[],
                const int& spread[])
  {
      if(rates_total <= MINBAR) return 0;
      int limit = rates_total - prev_calculated;
      if (limit == 0)        {
      } else if (limit == 1) { 
         GetValue(high, low, 1);
      } else if (limit > 1)  { 
         ArrayInitialize(v3,EMPTY_VALUE);
         ArrayInitialize(trigger,EMPTY_VALUE);
         ArrayInitialize(v1,0);
         ArrayInitialize(v2,0);
         ArrayInitialize(sg,0);
         limit = rates_total - MINBAR;
         for(int i = limit; i >= 1 && !IsStopped(); i--){
            GetValue(high, low, i);
         }//for(int i = limit + 1; i >= 0 && !IsStopped(); i--)
         return(rates_total);         
      }
      GetValue(high, low, 0); 

   return(rates_total);
  }
//+------------------------------------------------------------------+
