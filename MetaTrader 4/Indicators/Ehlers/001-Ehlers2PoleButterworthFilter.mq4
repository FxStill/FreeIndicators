//+------------------------------------------------------------------+
//|                          Ehlers2PoleButterworthFilter.mq5    |
//|                                Copyright 2020, Andrei Novichkov. |
//|  Main Site: http://fxstill.com                                   |
//|  Telegram:  https://t.me/fxstill (Literature on cryptocurrencies,| 
//|                                   development and code. )        |
//+------------------------------------------------------------------+
#property copyright "Copyright 2020, Andrei Novichkov."
#property link      "http://fxstill.com"
#property version   "1.00"
#property description "Telegram Channel: https://t.me/fxstill\n"
#property description "The 2 Pole Butterworth Filter:\nJohn Ehlers, \"Cybernetic Analysis For Stocks And Futures\", pg.192"

#property strict

#property indicator_chart_window

#property indicator_buffers 1


#property indicator_type1   DRAW_LINE 
#property indicator_width1  2
#property indicator_style1  STYLE_SOLID
#property indicator_color1  clrGreen

input int length = 15;

double bf[];

static const int MINBAR = 5;

double a, b, c1, c2, c3;
int OnInit() {

   SetIndexBuffer(0,bf,INDICATOR_DATA);
   
   IndicatorSetString(INDICATOR_SHORTNAME,"Ehlers2PoleButterworthFilter");
   IndicatorSetInteger(INDICATOR_DIGITS,_Digits);

   a = MathExp(-M_SQRT2 * M_PI / length);
   b = 2 * a * MathCos(M_SQRT2 * M_PI / length);
   c2 = b;
   c3 = - MathPow(a, 2);
   c1 = (1 - b + MathPow(a, 2)) / 4;
   
   return INIT__SUCCEEDED();
}

void GetValue(const double& h[], const double& l[], int shift) {

   double b1 = ZerroIfEmpty(bf[shift + 1]);
   double b2 = ZerroIfEmpty(bf[shift + 2]);
   double p0 = (h[shift] + l[shift]) / 2;
   double p1 = (h[shift + 1] + l[shift + 1]) / 2;
   double p3 = (h[shift + 3] + l[shift + 3]) / 2;
   bf[shift] = c1 * (p0 + 2 * p1 + p3) +  c2 * b1 + c3 * b2;
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
         ArrayInitialize(bf,EMPTY_VALUE);
         limit = rates_total - MINBAR;
         for(int i = limit; i >= 1 && !IsStopped(); i--){
            GetValue(high, low, i);
         }//for(int i = limit + 1; i >= 0 && !IsStopped(); i--)
         return(rates_total);         
      }
      GetValue(high, low, 0);          
      return(rates_total);
}
