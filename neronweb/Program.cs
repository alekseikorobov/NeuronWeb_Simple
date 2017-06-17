using System;

namespace neronweb
{
	class MainClass
	{
		//double[] enteres;
		int CountEnteres = 2;
		double[] hidden;
		//double[] outer;
		int CountOuter = 1;
		double[,] wEH; //веса от enteres до hidden
		double[,] wHO; //веса от hidden до outer

		double[,] patterns = {
			{0,0},{1,0},{0,1},{1,1}
		};
		double[] answers = {0,1,1,0};

		public MainClass ()
		{
			//enteres = new double[patterns.GetLength (1)];
			hidden = new double[2];
			//outer = new double[1];
			wEH = new double[CountEnteres, hidden.Length];
			wHO = new double[hidden.Length,CountOuter]; //1 колличество выходов
		}

		public void init()
		{
			Random r = new Random ();
			for (int i = 0; i < wEH.GetLength(0); i++) {
				for (int j = 0; j < wEH.GetLength(1); j++) {
					wEH [i, j] = r.GetRandomNumber(0.1, 0.3);
				}
			}
			for (int i = 0; i < wHO.GetLength(0); i++) {
				for (int j = 0; j < wHO.GetLength(1); j++) {					
					wHO [i,j] = r.GetRandomNumber (0.1f, 0.3);
				}
			}
		}

		public double[] countOuter(double[] enteres){

			for (int i = 0; i < hidden.Length; i++) {
				hidden [i] = 0;
				for (int j = 0; j < CountEnteres; j++) {
					hidden [i] += enteres [j] * wEH [j, i];
				}
				if (hidden [i] > 0.5)
					hidden [i] = 1;
				else
					hidden [i] = 0;
					
			}
			double[] outer = new double[CountOuter];
			for (int j = 0; j < CountOuter; j++) {				
				outer[j] = 0;
				for (int i = 0; i < hidden.Length; i++) {
					outer[j]  += hidden [i] * wHO [i,j];
				}
				if (outer[j] > 0.5)
					outer[j]  = 1;
				else
					outer[j]  = 0;
			}
			return outer;
		}

		public int study_old(){
			double[] err = new double[hidden.Length];
			double gError = 0;
			int iteration = 0;
			do {
				iteration++;
				gError = 0;
				for (int p = 0; p < patterns.GetLength(0) ; p++) {
					double[] enteres = new double[CountEnteres];
					for (int i = 0; i < CountEnteres; i++) {
						enteres[i] = patterns[p,i];
					}					
					double[] outer = countOuter(enteres);
					double lError = answers[p] - outer[0]; //дельта ошибки

					gError += Math.Abs(lError);

					for (int i = 0; i < hidden.Length; i++) {
						err[i] = 0;
						for (int j = 0; j < CountOuter; j++) {
							err[i] += lError * wHO[i,j];
						}
					}
					for (int i = 0; i < CountEnteres; i++) {
						for (int j = 0; j < hidden.Length; j++) {
							wEH[i,j] += 0.1 * err[j] * enteres[i];
						}
					}
					for (int j = 0; j < CountOuter; j++) {						
						for (int i = 0; i < hidden.Length; i++) {						
							wHO[i,j] += 0.1 * lError* hidden[i]; 
						}
					}

				}
			} while(gError != 0);
			return iteration;
		}
		double[] deltaError(double[] answers,double[] outer){
			double[] error = new double[outer.Length];

			for (int i = 0; i < outer.Length; i++) {
				error [i] = answers[i] - outer[i];
			}
			return error;
		}
		private double GetError(double[] enteres,double answers){
			double[] outer = countOuter(enteres);
			double[] lError = deltaError(new double[]{answers},outer); //дельта ошибки

			double[] err = new double[hidden.Length];
			double gError = Math.Abs(lError[0]);

			for (int i = 0; i < hidden.Length; i++) {
				err[i] = 0;
				for (int j = 0; j < CountOuter; j++) {
					err[i] += lError[j] * wHO[i,j];
				}
			}
			for (int i = 0; i < CountEnteres; i++) {
				for (int j = 0; j < hidden.Length; j++) {
					wEH[i,j] += 0.1 * err[j] * enteres[i];
				}
			}
			for (int j = 0; j < CountOuter; j++) {						
				for (int i = 0; i < hidden.Length; i++) {						
					wHO[i,j] += 0.1 * lError[j] * hidden[i]; 
				}
			}
			return gError;
		}

		public int study(){
			//
			double gError = 0;
			int iteration = 0;
			do {
				iteration++;
				gError = 0;
				for (int p = 0; p < patterns.GetLength(0) ; p++) {
					double[] enteres = new double[CountEnteres];
					for (int i = 0; i < CountEnteres; i++) {
						enteres[i] = patterns[p,i];
					}
					gError += GetError(enteres, answers[p]);
				}
			} while(gError != 0);
			return iteration;
		}

		public void test(){
			for (int p = 0; p < patterns.GetLength(0); p++) {
				double[] enteres = new double[CountEnteres];
				for (int i = 0; i < CountEnteres; i++) {
					enteres [i] = patterns [p,i];
				}					
				double[] outer = countOuter (enteres);

				for (int i = 0; i < CountOuter; i++) {
					Console.WriteLine (outer[i]);
				}
			}
		}

		public void studyAll()
		{
			for (int i = 0; i < patterns.GetLength(0); i++) {
				
			}
		}
		static MainClass m = new MainClass ();
		public static void Main (string[] args)
		{
			m.init ();
			int iteration = m.study ();

			m.test ();

			Console.WriteLine ("iteration - {0}",iteration);
		}
	}

	public static class ExtentionRandom{
		public static double GetRandomNumber(this Random r,double minimum, double maximum)
		{ 
			return r.NextDouble() * (maximum - minimum) + minimum;
		}
	}
}
