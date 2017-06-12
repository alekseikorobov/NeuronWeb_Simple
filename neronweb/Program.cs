using System;

namespace neronweb
{
	class MainClass
	{
		double[] enteres;
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
			enteres = new double[patterns.GetLength (1)];
			hidden = new double[2];
			//outer = new double[1];
			wEH = new double[enteres.Length, hidden.Length];
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

		public double[] countOuter(){

			for (int i = 0; i < hidden.Length; i++) {
				hidden [i] = 0;
				for (int j = 0; j < enteres.Length; j++) {
					hidden [i] += enteres [j] * wEH [j, i];
				}
				if (hidden [i] > 0.5)
					hidden [i] = 1;
				else
					hidden [i] = 0;
					
			}
			double[] outer = new double[CountOuter];
			for (int j = 0; j < outer.Length; j++) {				
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

		public int study(){
			double[] err = new double[hidden.Length];
			double gError = 0;
			int iteration = 0;
			do {
				iteration++;
				gError = 0;
				for (int p = 0; p < patterns.GetLength(0) ; p++) {
					for (int i = 0; i < enteres.Length; i++) {
						enteres[i] = patterns[p,i];
					}					
					double[] outer = countOuter();
					double lError = answers[p] - outer[0]; //дельта ошибки

					gError += Math.Abs(lError);

					for (int i = 0; i < hidden.Length; i++) {
						err[i] = 0;
						for (int j = 0; j < outer.Length; j++) {
							err[i] += lError * wHO[i,j];
						}
					}
					for (int i = 0; i < enteres.Length; i++) {
						for (int j = 0; j < hidden.Length; j++) {
							wEH[i,j] += 0.1 * err[j] * enteres[i];
						}
					}
					for (int j = 0; j < outer.Length; j++) {						
						for (int i = 0; i < hidden.Length; i++) {						
							wHO[i,j] += 0.1 * lError* hidden[i]; 
						}
					}

				}
			} while(gError != 0);
			return iteration;
		}

		public void test(){
			for (int p = 0; p < patterns.GetLength(0); p++) {
				for (int i = 0; i < enteres.Length; i++) {
					enteres [i] = patterns [p,i];
				}					
				double[] outer = countOuter ();

				for (int i = 0; i < outer.Length; i++) {
					Console.WriteLine (outer[i]);
				}
			}
		}

		public static void Main (string[] args)
		{
			MainClass m = new MainClass ();

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
