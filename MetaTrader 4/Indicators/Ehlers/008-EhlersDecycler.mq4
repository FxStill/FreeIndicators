//+------------------------------------------------------------------+
//|                                               EhlersDecycler.mq5 |
//|                                Copyright 2020, Andrei Novichkov. |
//|  Main Site: http://fxstill.com                                   |
//|  Telegram:  https://t.me/fxstill (Literature on cryptocurrencies,| 
//|                                   development and code. )        |
//+------------------------------------------------------------------+
#property copyright "Copyright 2020, Andrei Novichkov."
#property link      "http://fxstill.com"
#property version   "1.00"
#property description "Telegram Channel: https://t.me/fxstill\n"
#property description "The Decycler:\nJohn Ehlers, \"Cycle Analytics For Traders\", pg.40-41"


#property indicator_chart_window
#property indicator_buffers 1
//--- plot db
#property indicator_label1  "db"
#property indicator_type1   DRAW_LINE
#property indicator_color1  clrGreen,clrRed,clrLimeGreen
#property indicator_style1  STYLE_SOLID
#property indicator_width1  2
//--- input parameters
input int      length = 20; //Length
//--- indicator buffers
double         db[];
double a, a1, t;
static const int MINBAR = 5;
//+------------------------------------------------------------------+
//| Custom indicator initialization function                         |
//+------------------------------------------------------------------+
int OnInit()
  {
//--- indicator buffers mapping
   SetIndexBuffer(0,db);
//---
   IndicatorSetString(INDICATOR_SHORTNAME,"EhlersDecycler");
   IndicatorSetInteger(INDICATOR_DIGITS,_Digits);

    t = 2 * M_PI / length;
    a = (MathCos(t) + MathSin(t) - 1) / MathCos(t);
    a1 = 1 - a;
    a /= 2;
   return INIT__SUCCEEDED();
  }
  
void GetValue(const double& h[], const double& l[], int shift) {

   double p0 = (h[shift] + l[shift]) / 2;
   double p1 = (h[shift + 1] + l[shift + 1]) / 2;  
   double d1 = ZerroIfEmpty(db[shift + 1]);
   
   db[shift] = a * (p0 + p1) + a1 * d1;
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
         ArrayInitialize(db,EMPTY_VALUE);
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
