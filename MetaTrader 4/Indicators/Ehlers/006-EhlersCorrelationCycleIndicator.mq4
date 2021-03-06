//+------------------------------------------------------------------+
//|                              EhlersCorrelationCycleIndicator.mq5 |
//|                                Copyright 2020, Andrei Novichkov. |
//|  Main Site: http://fxstill.com                                   |
//|  Telegram:  https://t.me/fxstill (Literature on cryptocurrencies,| 
//|                                   development and code. )        |
//+------------------------------------------------------------------+
#property copyright "Copyright 2020, Andrei Novichkov."
#property link      "http://fxstill.com"
#property version   "1.00"
#property description "Telegram Channel: https://t.me/fxstill\n"
#property description "The Correlation Cycle Indicator:\nJohn Ehlers, \"Stocks & Commodities V. 38:06 (8–15)\""


#define NAME (string)"EhlersCorrelationCycleIndicator"

#property indicator_separate_window

#property indicator_buffers 4

#property indicator_label1  "Real"
#property indicator_type1   DRAW_LINE
#property indicator_color1  clrGreen
#property indicator_style1  STYLE_SOLID
#property indicator_width1  2

#property indicator_label2  "Imag"
#property indicator_type2   DRAW_LINE
#property indicator_color2  clrDodgerBlue
#property indicator_style2  STYLE_SOLID
#property indicator_width2  2

#property indicator_label3  "Angle"
#property indicator_type3   DRAW_LINE
#property indicator_color3  clrOrchid
#property indicator_style3  STYLE_SOLID
#property indicator_width3  2

#property indicator_label4  "State"
#property indicator_type4   DRAW_LINE
#property indicator_color4  clrOrange
#property indicator_style4  STYLE_SOLID
#property indicator_width4  2


input int  length = 20;       // Length
input bool ViewAngle = false; //View Phasor
input bool ViewState = false; //View Market State

//--- indicator buffers
double         real[];
double         imag[];
double         Angle[];
double         State[];

static const int MINBAR = length + 1;
double pi2, pi05, pi32, pi9;
//+------------------------------------------------------------------+
//| Custom indicator initialization function                         |
//+------------------------------------------------------------------+
int OnInit()   {
   
//--- indicator buffers mapping
   SetIndexBuffer(0,real);
   SetIndexBuffer(1,imag);
   SetIndexBuffer(2,Angle);
   SetIndexBuffer(3,State);
   
   IndicatorShortName(NAME);
   IndicatorDigits(Digits);   
//---
	pi2  = M_PI * 2;
	pi05 = M_PI_2;
	pi32 = M_PI * 3 / 2;
	pi9  = 9 * M_PI / 180;
   return INIT__SUCCEEDED();
  }

  
  
void GetValue(const double& h[], const double& l[], int shift) {

   double sx = 0.0, sy = 0.0, sxx = 0.0, syy = 0.0, sxy = 0.0, nsy = 0.0, nsyy = 0.0, nsxy = 0.0;
   double x, y, ny;
   
   for(int i = 1; i <= length; i++) {
      x = (h[shift + i - 1] + l[shift + i - 1]) / 2;
      y =   MathCos(pi2 * (i - 1) / length);
      ny = -MathSin(pi2 * (i - 1) / length);
      
      sx   = sx + x;
      sy   = sy + y;
      nsy  = nsy + ny;
      sxx  = sxx + MathPow(x, 2);
      syy  = syy + MathPow(y, 2);
      nsyy = nsyy + MathPow(ny, 2);
      sxy  = sxy + x * y;
      nsxy = nsxy + x * ny;
   }// for(int i = 1; i < length; i++)
   
   if ( (length * sxx - MathPow(sx, 2) > 0) && (length * syy - MathPow(sy, 2) > 0) )
      real[shift] = (length * sxy - sx * sy) / MathSqrt((length * sxx - MathPow(sx, 2)) * (length * syy - sx * sy ));
   else real[shift] = 0;   
   
   if ( (length * sxx - MathPow(sx, 2) > 0) && (length * nsyy - MathPow(nsy, 2) > 0) )
      imag[shift] = (length * nsxy - sx * nsy) / MathSqrt((length * sxx - MathPow(sx, 2)) * (length * nsyy - MathPow(nsy, 2)));
   else imag[shift] = 0;   
   
	if (ViewAngle) {
		if (imag[shift] != 0.0)  
			Angle[shift] = pi05 + MathArctan(real[shift] / imag[shift]);
		if (imag[shift] > 0)  
			Angle[shift] = Angle[shift] - M_PI;
		if (Angle[shift + 1] - Angle[shift] < pi32 && Angle[shift] < Angle[shift + 1]) 
			Angle[shift] = Angle[shift + 1];
		if (ViewState) {
			State[shift] = 0;
			if(MathAbs(Angle[shift] - Angle[shift + 1]) < pi9){
				 State[shift] = (Angle[shift] < 0)? -1: 1;
			}// if(Math.Abs(Angle[shift] - Angle[1]) < pi9)
		}// if (ViewState)
	}// if (ViewAngle)   
   
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
         ArrayInitialize(real, EMPTY_VALUE);
         ArrayInitialize(imag, EMPTY_VALUE);
         ArrayInitialize(Angle, EMPTY_VALUE);
         ArrayInitialize(State, EMPTY_VALUE);
         limit = rates_total - MINBAR;
         for(int i = limit; i >= 1 && !IsStopped(); i--){
            GetValue(high, low, i);
         }//for(int i = limit + 1; i >= 0 && !IsStopped(); i--)
         return(rates_total);         
      }
      GetValue(high, low, 0);          
   return(rates_total);
}
