//+------------------------------------------------------------------+
//|                               Ehlers2PoleSuperSmootherFilter.mq5 |
//|                                Copyright 2020, Andrei Novichkov. |
//|  Main Site: http://fxstill.com                                   |
//|  Telegram:  https://t.me/fxstill (Literature on cryptocurrencies,| 
//|                                   development and code. )        |
//+------------------------------------------------------------------+
#property copyright "Copyright 2020, Andrei Novichkov."
#property link      "http://fxstill.com"
#property version   "1.00"
#property description "Telegram Channel: https://t.me/fxstill\n"
#property description "The 2 Pole Super Smoother Filter:\nJohn Ehlers, \"Cybernetic Analysis For Stocks And Futures\", pg.202"

#property indicator_chart_window

#property indicator_buffers 1
//--- plot ssf
#property indicator_label1  "ssf"
#property indicator_type1   DRAW_LINE
#property indicator_color1  clrLimeGreen
#property indicator_style1  STYLE_SOLID
#property indicator_width1  2

input int length = 15; //Length
//--- indicator buffers
double         sb[];

static const int MINBAR = 3;
//+------------------------------------------------------------------+
//| Custom indicator initialization function                         |
//+------------------------------------------------------------------+
double c1, c2, c3;
int OnInit() {
//--- indicator buffers mapping
   SetIndexBuffer(0,sb,INDICATOR_DATA);
   
   IndicatorSetString(INDICATOR_SHORTNAME,"Ehlers2PoleSuperSmootherFilter");
   IndicatorSetInteger(INDICATOR_DIGITS,_Digits);   

   double a1 = MathExp(-M_SQRT2 * M_PI / length);
   double b1 = 2 * a1 * MathCos(M_SQRT2 * M_PI / length);
   c2 = b1;
   c3 = -MathPow(a1, 2);
   c1 = 1 - c2 - c3;

   return INIT__SUCCEEDED();
}

void GetValue(const double& h[], const double& l[], int shift) {

   double s1 = ZerroIfEmpty(sb[shift + 1]);
   double s2 = ZerroIfEmpty(sb[shift + 2]);

   sb[shift] = c1 * (h[shift] + l[shift]) / 2 + c2 * s1 + c3 * s2;
   

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
      } else if (limit > 1)  {   // Первый вызов индикатора, смена таймфрейма, подгрузка данных из истории
         ArrayInitialize(sb,   EMPTY_VALUE);
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
