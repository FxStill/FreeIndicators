//+------------------------------------------------------------------+
//|                                EhlersSwissArmyKnifeIndicator.mq5 |
//|                                Copyright 2020, Andrei Novichkov. |
//|  Main Site: http://fxstill.com                                   |
//|  Telegram:  https://t.me/fxstill (Literature on cryptocurrencies,| 
//|                                   development and code. )        |
//+------------------------------------------------------------------+
#property copyright "Copyright 2020, Andrei Novichkov."
#property link      "http://fxstill.com"
#property version   "1.00"
#property description "Telegram Channel: https://t.me/fxstill\n"
#property description "The Swiss Army Knife Indicator:\nJohn Ehlers, \"Stocks & Commodities V. 24:1 (28-31, 50-53)\""

#define NAME (string)"EhlersSwissArmyKnifeIndicator"

#property indicator_separate_window

#property indicator_buffers 2
//--- plot snr
#property indicator_label1  "Saki"
#property indicator_type1   DRAW_LINE
#property indicator_color1  clrLimeGreen
#property indicator_style1  STYLE_SOLID
#property indicator_width1  2

#property indicator_label2  ""
#property indicator_type2   DRAW_NONE
//

enum TSMOOTH {
   Ema, 
   Sma, 
   Gauss, 
   Butter, 
   Smooth, 
   Hp, 
   Php2, 
   Bp, 
   Bs
};

input TSMOOTH type = Hp;  //Indicator Type
input int length   = 20;  //Length
input double delta = 0.1; //Delta

//--- indicator buffers
double         saki[];
double         v1[];

static const int MINBAR = length + 1;
double c0 = 1.0, c1 = 0.0, b0 = 1.0, b1 = 0.0, b2 = 0.0, a1 = 0.0, a2 = 0.0;

//+------------------------------------------------------------------+
//| Custom indicator initialization function                         |
//+------------------------------------------------------------------+
int OnInit()   {
   
//--- indicator buffers mapping
   SetIndexBuffer(0,saki);
   SetIndexBuffer(1,v1);

   IndicatorSetString(INDICATOR_SHORTNAME, NAME);
   IndicatorSetInteger(INDICATOR_DIGITS,_Digits);   
   
   double alpha = 0.0, beta = 0.0, gamma = 0.0;
   
   double twoPiPrd = 2 * M_PI / length;
   double fourPiPrd = 4 * M_PI * delta / length ;
   switch(type){
      case Ema: 
         alpha = (MathCos(twoPiPrd) + MathSin(twoPiPrd) - 1) / MathCos(twoPiPrd);
         b0 = alpha;
         a1 = 1 - alpha;      
         break;
      case Sma:
         c1 = 1.0 / length;
         b0 = 1.0 / length;
         a1 = 1;    
         break; 
      case Gauss: 
         beta = 2.415 * (1 - MathCos(twoPiPrd));
         alpha = -beta + MathSqrt(MathPow(beta,2) + (2 * beta));
         c0 = MathPow(alpha,2);
         a1 = 2.0 * (1.0 - alpha);
         a2 = -MathPow(1.0 - alpha, 2);
         break; 
      case Butter: 
         beta = 2.415 * (1.0 - MathCos(twoPiPrd));
         alpha = -beta + MathSqrt(MathPow(beta,2) + (2 * beta));
         c0 = MathPow(alpha,2) / 4.0;
         b1 = 2.0;
         b2 = 1.0;
         a1 = 2.0 * (1 - alpha);
         a2 = -MathPow(1.0 - alpha, 2);
         break; 
      case Smooth: 
         c0 = 0.25;
         b1 = 2.0;
         b2 = 1.0;      
         break; 
      case Hp: 
         alpha = (MathCos(twoPiPrd) + MathSin(twoPiPrd) - 1) / MathCos(twoPiPrd);
         c0 = 1.0 - alpha / 2.0;
         b1 = -1.0;
         a1 = 1.0 - alpha;
         break; 
      case Php2: 
         beta = 2.415 * (1.0 - MathCos(twoPiPrd));
         alpha = -beta + MathSqrt(MathPow(beta,2) + (2.0 * beta));
         c0 = MathPow(1 - alpha / 2.0, 2);
         b1 = -2.0;
         b2 = 1.0;
         a1 = 2.0 * (1.0 - alpha);
         a2 = -MathPow(1.0 - alpha, 2);
         break; 
      case Bp: 
         beta = MathCos(twoPiPrd);
         gamma = 1.0 / MathCos(fourPiPrd);
         alpha = gamma - MathSqrt(MathPow(gamma,2) - 1.0);
         c0 = (1 - alpha) / 2;
         b2 = -1.0;
         a1 = beta * (1.0 + alpha);
         a2 = -alpha;      
         break; 
      case Bs: 
         beta = MathCos(twoPiPrd);
         gamma = 1.0 / MathCos(fourPiPrd);
         alpha = gamma - MathSqrt(MathPow(gamma,2) - 1.0);
         c0 = (1.0 + alpha) / 2.0;
         b1 = -2.0 * beta;
         b2 = 1.0;
         a1 = beta * (1.0 + alpha);
         a2 = -alpha;      
         break; 
   }
//---
   return INIT__SUCCEEDED();
  }

  
  
void GetValue(const double& h[], const double& l[], int shift) {

   double s1 = ZerroIfEmpty(saki[shift + 1]);
   double s2 = ZerroIfEmpty(saki[shift + 2]);
   double price0 = (h[shift] + l[shift]) / 2;
   double price1 = (h[shift + 1] + l[shift + 1]) / 2;
   double price2 = (h[shift + 2] + l[shift + 2]) / 2;
   double pricep = (h[shift + length] + l[shift + length]) / 2;   
   saki[shift] = c0 * (b0 * price0 + b1 * price1 + b2 * price2) + a1 * s1 + a2 * s2 - c1 * pricep;
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
      if(rates_total < MINBAR) return 0;
      int limit = rates_total - prev_calculated;
      if (limit == 0)        {   // Пришел новый тик 
      } else if (limit == 1) {   // Образовался новый бар
         GetValue(high, low, 1);
         return(rates_total); 
      } else if (limit > 1)  {   // Первый вызов индикатора, смена таймфрейма, подгрузка данных из истории
         ArrayInitialize(saki, EMPTY_VALUE);
         ArrayInitialize(v1, 0);
         limit = rates_total - MINBAR;
         for(int i = limit; i >= 1 && !IsStopped(); i--){
            GetValue(high, low, i);
         }//for(int i = limit + 1; i >= 0 && !IsStopped(); i--)
         return(rates_total);         
      }
      GetValue(high, low, 0);          
   return(rates_total);
}
