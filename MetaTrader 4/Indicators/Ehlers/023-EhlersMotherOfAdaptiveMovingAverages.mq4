//+------------------------------------------------------------------+
//|                         EhlersMotherOfAdaptiveMovingAverages.mq5 |
//|                                Copyright 2020, Andrei Novichkov. |
//|  Main Site: http://fxstill.com                                   |
//|  Telegram:  https://t.me/fxstill (Literature on cryptocurrencies,| 
//|                                   development and code. )        |
//+------------------------------------------------------------------+
#property copyright "Copyright 2020, Andrei Novichkov."
#property link      "http://fxstill.com"
#property version   "1.00"
#property description "Telegram Channel: https://t.me/fxstill\n"
#property description "The Mother Of Adaptive Moving Averages:\nJohn Ehlers, \"Rocket Science For Traders\", pg.182-183"


#property indicator_chart_window
#property indicator_buffers 3
//--- plot mama
#property indicator_label1  "mama"
#property indicator_type1   DRAW_LINE
#property indicator_color1  clrLimeGreen
#property indicator_style1  STYLE_SOLID
#property indicator_width1  2
//--- plot fama
#property indicator_label2  "fama"
#property indicator_type2   DRAW_LINE
#property indicator_color2  clrRed
#property indicator_style2  STYLE_SOLID
#property indicator_width2  2

#property indicator_label3  ""
#property indicator_type3   DRAW_NONE
//--- input parameters
input double   FastLimit = 0.5;
input double   SlowLimit = 0.05;
//--- indicator buffers
double         mb[];
double         fb[];
double         phase[];

static const int MINBAR = 5;

//+------------------------------------------------------------------+
//| Custom indicator initialization function                         |
//+------------------------------------------------------------------+
int OnInit() {
//--- indicator buffers mapping
   SetIndexBuffer(0,mb);
   SetIndexBuffer(1,fb);
   SetIndexBuffer(2,phase);

   IndicatorSetString(INDICATOR_SHORTNAME,"EhlersMotherOfAdaptiveMovingAverages");
   IndicatorSetInteger(INDICATOR_DIGITS,_Digits);  
   
//---
   return INIT__SUCCEEDED();
  }
  
void GetValue(const double& h[], const double& l[], int shift) {
   double i1 = iCustom(NULL,0,"EhlersHilbertTransform",4,shift);
   double q1 = iCustom(NULL,0,"EhlersHilbertTransform",5,shift);
   
   phase[shift] = (i1 != 0)? MathArctan(q1 / i1) : 0;
   double deltaPhase = phase[shift + 1] - phase[shift];
   deltaPhase = (deltaPhase < 1)? 1 : deltaPhase;

   double alpha = FastLimit / deltaPhase;
   alpha = (alpha < SlowLimit)? SlowLimit : alpha;

   double m = ZerroIfEmpty(mb[shift + 1]);
   double f = ZerroIfEmpty(fb[shift + 1]);
   
   mb[shift] = alpha * (h[shift] + l[shift]) / 2 + (1 - alpha) * m;
   fb[shift] = 0.5 * alpha * mb[shift] + (1 - 0.5 * alpha) * f;   

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
      if (limit == 0)        {   // Пришел новый тик 
      } else if (limit == 1) {   // Образовался новый бар
         GetValue(high, low, 1);          
         return(rates_total);      
      } else if (limit > 1)  {   // Первый вызов индикатора, смена таймфрейма, подгрузка данных из истории
         ArrayInitialize(mb,   EMPTY_VALUE);
         ArrayInitialize(fb,   EMPTY_VALUE);
         ArrayInitialize(phase, 0);
         limit = rates_total - MINBAR;
         for(int i = limit; i >= 1 && !IsStopped(); i--){
            GetValue(high, low, i);
         }//for(int i = limit + 1; i >= 0 && !IsStopped(); i--)
         return(rates_total);         
      }
      GetValue(high, low, 0);          

   return(rates_total);
  }
  