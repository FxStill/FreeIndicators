//+------------------------------------------------------------------+
//|                                             EhlersCyberCycle.mq5 |
//|                                Copyright 2020, Andrei Novichkov. |
//|                                               http://fxstill.com |
//|   Telegram: https://t.me/fxstill (Literature on cryptocurrencies,|
//|                                   development and code. )        |
//+------------------------------------------------------------------+
#property copyright "Copyright 2020, Andrei Novichkov."
#property link      "http://fxstill.com"
#property version   "1.00"

#property description "Telegram Channel: https://t.me/fxstill\n"
#property description "The Cyber Cycle:\nJohn Ehlers, \"Cybernetic Analysis For Stocks And Futures\", pg.34"

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

#property indicator_label3  ""
#property indicator_type3   DRAW_NONE

input double alpha = 0.07;//Alpha

double cycle[], trigger[], smooth[];

double a1, a2;

static const int MINBAR = 5;

int OnInit()  {

   SetIndexBuffer(0,cycle);
   SetIndexBuffer(1,trigger);
   SetIndexBuffer(2,smooth); 
   
   IndicatorSetString(INDICATOR_SHORTNAME,"EhlersSuperPassBandFilter");
   IndicatorSetInteger(INDICATOR_DIGITS,_Digits);

   return(INIT_SUCCEEDED);
}

void GetValue(const double& h[], const double& l[], int shift) {

   double p0 = (h[shift] + l[shift]) / 2;
   double p1 = (h[shift + 1] + l[shift + 1]) / 2;
   double p2 = (h[shift + 2] + l[shift + 2]) / 2;
   double p3 = (h[shift + 3] + l[shift + 3]) / 2;   
   smooth[shift] = (p0 + 2 * p1 + 2 * p2 + p3) / 6;
   if (false) {
      cycle[shift] = (p0 - 2 * p1 + p2) / 4;
   } else {
      double c1 = ZerroIfEmpty(cycle[shift + 1]);
      double c2 = ZerroIfEmpty(cycle[shift + 2]);
      cycle[shift] = pow(1 - (0.5 * alpha), 2) * (smooth[shift] - 2 * smooth[shift + 1] + smooth[shift + 2]) + 
                     2 * (1 - alpha) * c1 - pow(1 - alpha, 2) * c2;
   }
   trigger[shift] = cycle[shift + 1];
            
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
      } else if (limit > 1)  {   // Первый вызов индикатора, смена таймфрейма, подгрузка данных из истории
         ArrayInitialize(cycle,   EMPTY_VALUE);
         ArrayInitialize(trigger, EMPTY_VALUE);
         ArrayInitialize(smooth,  0);
         limit = rates_total - MINBAR;
         for(int i = limit; i >= 1 && !IsStopped(); i--){
            GetValue(high, low, i);
         }//for(int i = limit + 1; i >= 0 && !IsStopped(); i--)
         return(rates_total);         
      }
      GetValue(high, low, 0);          
                
   return(rates_total);
}

