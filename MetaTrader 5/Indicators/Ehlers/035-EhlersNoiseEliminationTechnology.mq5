//+------------------------------------------------------------------+
//|                          EhlersNoiseEliminationTechnology.mq5    |
//|                                Copyright 2021, Andrei Novichkov. |
//|                                               http://fxstill.com |
//|   Telegram: https://t.me/fxstill (Literature on cryptocurrencies,| 
//|                                   development and code. )        |
//+------------------------------------------------------------------+
#property copyright "Copyright 2020, Andrei Novichkov."
#property link      "https://fxstill.com"

#property version   "1.00"

#property description "Telegram Channel: https://t.me/fxstill\n"
#property description "Noise Elimination Technology: John Ehlers, \"Stocks & Commodities. Dec.2020 pg. 16-18\""

#property indicator_separate_window

#define NAME (string)"EhlersNoiseEliminationTechnology"

#property indicator_buffers 1
#property indicator_plots   1

//--- plot NET_Ehlers
#property indicator_label1  "NET_Ehlers"
#property indicator_type1   DRAW_LINE
#property indicator_color1  clrRed
#property indicator_style1  STYLE_SOLID
#property indicator_width1  1
//--- input parameters
input int      per    = 14;   //Period
input double   multi  = 1.0;
//--- indicator buffers
double         net[];

static int MINBAR;

double a1[], a2[];
double Denom;
int      period;
//+------------------------------------------------------------------+
//| Custom indicator initialization function                         |
//+------------------------------------------------------------------+
int OnInit() {

   period = (per == 0)? 14: per;
   MINBAR = period;
   
//--- indicator buffers mapping
   SetIndexBuffer(0,net,INDICATOR_DATA);
   
   ArraySetAsSeries(net,true);
   
   IndicatorSetString(INDICATOR_SHORTNAME, NAME);
   IndicatorSetInteger(INDICATOR_DIGITS,_Digits);     
   
   ArrayResize(a1, period);
   ArrayResize(a2, period);
   
   Denom = 0.5 * period * (period - 1);
   
   return INIT__SUCCEEDED();
  }
  
void GetValue(const double& price[], int shift) {
   ArrayInitialize(a1, 0);
   ArrayInitialize(a2, 0);

	for (int i = 1; i < period; i++) {
	   a1[i] = price[shift + i - 1];
		a2[i] = -i;
	}   
	double Num = 0;
	for (int i = 2; i < period; i++) {
				for (int j = 1; j < i - 1; j++)
					Num = Num - sign(a1[i] - a1[j]);
	}
	net[shift] = multi * Num / Denom;	
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
                const int begin,
                const double &price[])
  {
      if(rates_total < MINBAR) return 0;
      ArraySetAsSeries(price,true);    
      int limit = rates_total - prev_calculated;
      if (limit == 0)        {
      } else if (limit == 1) {
         GetValue(price, 1);
         return(rates_total);   
      } else if (limit > 1)  {
         ArrayInitialize(net,   EMPTY_VALUE);
         limit = rates_total - MINBAR;
         for(int i = limit; i >= 1 && !IsStopped(); i--){
            GetValue(price, i);
         }//for(int i = limit + 1; i >= 0 && !IsStopped(); i--)
         return(rates_total);         
      }
      GetValue(price, 0);          
   return(rates_total);
  }
//+------------------------------------------------------------------+

int sign(double x) {
   if (x == 0.0) return 0;
   if (x >  0.0) return 1;
   return -1;
}