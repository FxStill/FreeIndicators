//+------------------------------------------------------------------+
//|                                          EhlersRoofingFilter.mq5 |
//|                                Copyright 2020, Andrei Novichkov. |
//|                                               http://fxstill.com |
//|  Main Site: http://fxstill.com                                   |
//|  Telegram:  https://t.me/fxstill (Literature on cryptocurrencies,| 
//|                                   development and code. )        |
//+------------------------------------------------------------------+
#property copyright "Copyright 2020, Andrei Novichkov."
#property link      "http://fxstill.com"
#property version   "1.00"
#property description "Telegram Channel: https://t.me/fxstill\n"
#property description "Ehlers Roofing Filter:\nJohn Ehlers, \"Cycle Analytics for Traders\", pg.80-82"

#property indicator_separate_window

#property indicator_buffers 3
//--- plot rfilt
#property indicator_label1  "rfilt"
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
//--- input parameters
input int      hpLength=80;
input int      lpLength=40;
//--- indicator buffers
double         rb[];
double         tb[];
double         hp[];

static const int MINBAR = 5;

double c1, c2, c3;
double a1;
double t1, t2, t3;
//+------------------------------------------------------------------+
//| Custom indicator initialization function                         |
//+------------------------------------------------------------------+
int OnInit()
  {
//--- indicator buffers mapping
   SetIndexBuffer(0,rb);
   SetIndexBuffer(1,tb);
   SetIndexBuffer(2,hp);
   
   IndicatorSetString(INDICATOR_SHORTNAME,"EhlersRoofingFilter");
   IndicatorSetInteger(INDICATOR_DIGITS,_Digits);    
   
   double twoPiPrd = M_SQRT1_2 * 2 * M_PI / hpLength;
   a1 = (MathCos(twoPiPrd) + MathSin(twoPiPrd) - 1) / MathCos(twoPiPrd);
   double a2 = MathExp(-M_SQRT2 * M_PI / lpLength);
   double beta = 2 * a2 * MathCos(M_SQRT2 * M_PI / lpLength);
   c2 = beta;
   c3 = -a2 * a2;
   c1 = 1 - c2 - c3;
   t1 = MathPow(1 - (a1 / 2), 2);
   t2 = 1 - a1;
   t3 = MathPow(t2, 2);
   t2 *= 2;
   return INIT__SUCCEEDED();
  }
  
void GetValue(const double& price[], int shift) {

   hp[shift] =  t1 * (price[shift] - 2 * price[shift + 1] + price[shift + 2]) + 
                t2 * hp[shift + 1] - t3 * hp[shift + 2];

   double r1 = ZerroIfEmpty(rb[shift + 1]);               
   double r2 = ZerroIfEmpty(rb[shift + 2]);               
   
   rb[shift] = c1 * ((hp[shift] + hp[shift + 1]) / 2) + c2 * r1 + c3 * r2;
   tb[shift] = r2;
   
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
                const long &tick_volume[],
                const long &volume[],
                const int &spread[])
  {
      if(rates_total <= MINBAR) return 0;
      int limit = rates_total - prev_calculated;
      if (limit == 0)        {   // Пришел новый тик 
      } else if (limit == 1) {   // Образовался новый бар
         GetValue(close, 1);  
         return(rates_total);        
      } else if (limit > 1)  {   // Первый вызов индикатора, смена таймфрейма, подгрузка данных из истории
         ArrayInitialize(rb,EMPTY_VALUE);
         ArrayInitialize(tb,EMPTY_VALUE);
         ArrayInitialize(hp,0);
         limit = rates_total - MINBAR;
         for(int i = limit; i >= 1 && !IsStopped(); i--){
            GetValue(close, i);
         }//for(int i = limit + 1; i >= 0 && !IsStopped(); i--)
         return(rates_total);         
      }
      GetValue(close, 0); 
   return(rates_total);
  }
//+------------------------------------------------------------------+
