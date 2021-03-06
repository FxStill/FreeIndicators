//+------------------------------------------------------------------+
//|                                             HilbertTransform.mq5 |
//|                                Copyright 2020, Andrei Novichkov. |
//|  Main Site: http://fxstill.com                                   |
//|  Telegram:  https://t.me/fxstill (Literature on cryptocurrencies,| 
//|                                   development and code. )        |
//+------------------------------------------------------------------+
#property copyright "Copyright 2020, Andrei Novichkov."
#property link      "http://fxstill.com"
#property version   "1.00"
#property description "Telegram Channel: https://t.me/fxstill\n"
#property description "The Hilbert Transform:\nJohn Ehlers, \"Rocket Science for Traders.\", pg.51"


#property indicator_separate_window

#property indicator_buffers 14

#property indicator_type1   DRAW_LINE
#property indicator_width1  1
#property indicator_style1  STYLE_SOLID
#property indicator_color1  clrOrange //InPhase

#property indicator_type2   DRAW_LINE
#property indicator_width2  1
#property indicator_style2  STYLE_SOLID
#property indicator_color2  clrGreen  //Quadrature

#property indicator_label3  ""
#property indicator_type3   DRAW_NONE

#property indicator_label4  ""
#property indicator_type4   DRAW_NONE

#property indicator_label5  ""
#property indicator_type5   DRAW_NONE

#property indicator_label6  ""
#property indicator_type6   DRAW_NONE

#property indicator_label7  ""
#property indicator_type7   DRAW_NONE

#property indicator_label8  ""
#property indicator_type8   DRAW_NONE

#property indicator_label9  ""
#property indicator_type9   DRAW_NONE

#property indicator_label10  ""
#property indicator_type10   DRAW_NONE

#property indicator_label11  ""
#property indicator_type11   DRAW_NONE

#property indicator_label12  ""
#property indicator_type12   DRAW_NONE

#property indicator_label13  ""
#property indicator_type13   DRAW_NONE

#property indicator_label14  ""
#property indicator_type14   DRAW_NONE

double InPhase[], Quadrature[];
double Smooth[], Detrender[], I1[], Q1[], jI[], jQ[], I2[], Q2[], Re[], Im[], Per[], SmoothPeriod[]; 


static const int MINBAR = 7;
//+------------------------------------------------------------------+
//| Custom indicator initialization function                         |
//+------------------------------------------------------------------+
int OnInit() {
   
   SetIndexBuffer(0,InPhase);
   SetIndexBuffer(1,Quadrature);
   SetIndexBuffer(2,Smooth);
   SetIndexBuffer(3,Detrender);
   SetIndexBuffer(4,I1);
   SetIndexBuffer(5,Q1);
   SetIndexBuffer(6,jI);
   SetIndexBuffer(7,jQ);
   SetIndexBuffer(8,I2);
   SetIndexBuffer(9,Q2);
   SetIndexBuffer(10,Re);
   SetIndexBuffer(11,Im);
   SetIndexBuffer(12,Per);
   SetIndexBuffer(13,SmoothPeriod);   

   IndicatorSetString(INDICATOR_SHORTNAME,"EhlersHilbertTransform");
   IndicatorSetInteger(INDICATOR_DIGITS,_Digits);

   return(INIT_SUCCEEDED);
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
         Hilbert(high, low, 1);
         return(rates_total);
      } else if (limit > 1)  {   // Первый вызов индикатора, смена таймфрейма, подгрузка данных из истории
         ArrayInitialize(InPhase,EMPTY_VALUE);
         ArrayInitialize(Quadrature,EMPTY_VALUE);
         ArrayInitialize(Smooth,0);
         ArrayInitialize(Detrender,0);
         ArrayInitialize(I1,0);
         ArrayInitialize(Q1,0);
         ArrayInitialize(jI,0);
         ArrayInitialize(jQ,0);
         ArrayInitialize(I2,0);
         ArrayInitialize(Q2,0);
         ArrayInitialize(Re,0);
         ArrayInitialize(Im,0);
         ArrayInitialize(Per,0);
         ArrayInitialize(SmoothPeriod,0);
         limit = rates_total - MINBAR;
         for(int i = limit; i >= 1 && !IsStopped(); i--) {
            Hilbert(high, low, i);
         }//for(int i=limit; i>=0 && !IsStopped(); i--)
         return(rates_total);
      }
      Hilbert(high, low, 0);
   return(rates_total);
  }
  
void Hilbert(const double& h[], const double& l[], int i) {

         double p0 = (h[i] + l[i]) / 2;
         double p1 = (h[i + 1] + l[i + 1]) / 2;
         double p2 = (h[i + 2] + l[i + 2]) / 2;
         double p3 = (h[i + 3] + l[i + 3]) / 2;  
         
         Smooth[i] = (4 * p0 + 3 * p1 + 2 * p2 + p3) / 10;
         Detrender[i] = (0.0962 * Smooth[i]        + 0.5769 * Smooth[i + 2]     - 
                         0.5769 * Smooth[i + 4]    - 0.0962 * Smooth[i + 6])    * (0.075 * Per[i + 1] + 0.54);
//InPhase and Quadrature components                         
			Q1[i]        = (0.0962 * Detrender[i]     + 0.5769 * Detrender[i + 2]  - 
			                0.5769 * Detrender[i + 4] - 0.0962 * Detrender[i + 6]) * (0.075 * Per[i + 1] + 0.54);			 
			I1[i]        = Detrender[i + 3];    
//Advance the phase of I1 and Q1 by 90 degrees 
			jI[i]        = (0.0962 * I1[i]            + 0.5769 * I1[i + 2]         - 
			                0.5769 * I1[i + 4]        - 0.0962 * I1[i + 6])        * (0.075 * Per[i + 1] + 0.54); 
			jQ[i]        = (0.0962 * Q1[i]            + 0.5769 * Q1[i + 2]         -
			                0.5769 * Q1[i + 4]        - 0.0962 * Q1[i + 6])        * (0.075 * Per[i + 1] + 0.54); 	
//Phasor Addition 
			I2[i]        =  I1[i] - jQ[i]; 
			Q2[i]        =  Q1[i] + jI[i]; 	
//Smooth the I and Q components before applying the discriminator 
			I2[i]        =  0.2 * I2[i] + 0.8 * I2[i  + 1]; 
			Q2[i]        =  0.2 * Q2[i] + 0.8 * Q2[i  + 1]; 		
//Homodyne Discriminator 
			Re[i]        = I2[i] * I2[i]      + Q2[i] * Q2[i + 1]; 
			Im[i]        = I2[i] * Q2[i + 1]  - Q2[i] * I2[i + 1]; 
			Re[i]        = 0.2   * Re[i]      + 0.8   * Re[i + 1]; 
			Im[i]        = 0.2   * Im[i]      + 0.8   * Im[i + 1]; 		
			
			if(Im[i]!=0 && Re[i]!=0)       Per[i]     = 2 * M_PI  / (/*rad2Deg - */MathArctan(Im[i] / Re[i]) ); 		
			else                           Per[i]     = 0; 
			if(Per[i] > 1.5  * Per[i + 1]) Per[i]     = 1.5  * Per[i + 1]; 			 
			if(Per[i] < 0.67 * Per[i + 1]) Per[i]     = 0.67 * Per[i + 1]; 			 
			if(Per[i] < 6)                 Per[i]     = 6; 			 
			if(Per[i] > 50)                Per[i]     = 50; 		

			Per[i]          = 0.2  * Per[i] + 0.8  * Per[i + 1]; 
			SmoothPeriod[i] = 0.33 * Per[i] + 0.67 * SmoothPeriod[i + 1]; 		
			
			InPhase[i]      = I1[i]; 
			Quadrature[i]   = Q1[i];	
//InPhase[i] = 0.5 * (Smooth[i] - Smooth[i+2]) * (0.1759 * SmoothPeriod[i] + 0.4607);			
//InPhase[i] = Smooth[i+2];
}// void Hilbert(i)