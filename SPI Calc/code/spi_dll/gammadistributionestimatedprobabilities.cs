using System;

namespace NationalDroughtMitigationCenter
{

    /*************************************************************************
    *
    *     These functions compute the Standardized Precipitation Index
    *     using an incomplete gamma distribution function to estimate
    *     probabilities.
    *
    *     Useful references are:
    *
    *     _Numerical Recipes in C_ by Flannery, Teukolsky and Vetterling
    *     Cambridge University Press, ISBN 0-521-35465-x 
    *
    *     _Handbook of Mathematical Functions_ by Abramowitz and Stegun
    *     Dover, Standard Book Number 486-61272-4
    *
    *************************************************************************/
    internal static class GammaDistributionEstimatedProbabilities
    {

        /* Maximum number of iterations, and bound on error */
        private const int _intMaxIterations = 100;
        private const double _dblEpsilon = 3.0e-7;


        #region internal static double inv_normal( double prob )
        /****************************************************************************
        *
        *   input prob; return z.
        *
        *   See Abromowitz and Stegun _Handbook of Mathematical Functions_, p. 933
        *
        ****************************************************************************/
        internal static double inv_normal( double prob )
        {

            double t, minus;
            double c0 = 2.515517;
            double c1 = 0.802853;
            double c2 = 0.010328;
            double d1 = 1.432788;
            double d2 = 0.189269;
            double d3 = 0.001308;

            if( prob > 0.5 )
            {
                minus = 1.0;
                prob = 1.0 - prob;
            }
            else
            {
                minus = -1.0;
            }

            if( prob < 0.0 )
            {
                throw new ArgumentOutOfRangeException( "prob", "Probability not in [0,1.0].");
                //fprintf (stderr, "Error in inv_normal(). Prob. not in [0,1.0].\n");
                //return ((double)0.0);
            }
            
            if( prob == 0.0 )
                return (9999.0 * minus);

            t = Math.Sqrt( Math.Log( 1.0 / (prob * prob) ) );

            return (minus * (t - ((((c2 * t) + c1) * t) + c0) /
                     ((((((d3 * t) + d2) * t) + d1) * t) + 1.0)));

        } 
        #endregion

        #region internal static int gamma_fit( double[] datarr, ref double alpha, ref double beta, ref double gamm, ref double pzero )
        /***************************************************************************
        *
        *  Estimate incomplete gamma parameters.
        *
        *  Input:
        *      datarr - Data array
        *
        *  Output:
        *      alpha, beta, gamma - gamma paarameters
        *      pzero - probability of zero.
        *
        *  Return:
        *      number of non zero items in datarr.
        *
        ****************************************************************************/
        internal static int gamma_fit( double[] datarr, ref double alpha, ref double beta, ref double gamm, ref double pzero )
        {

            if( datarr == null || datarr.Length <= 0 )
                return (0);

            double sum = 0.0;
            double sumlog = 0.0;
            double mn = 0;
            double nact = 0;

            pzero = 0.0;
            

            /*  compute sums */
            for( int i = 0; i < datarr.Length; i++ )
            {
                if( datarr[i] > 0.0 )
                {
                    sum += datarr[i];
                    sumlog += Math.Log( datarr[i] );
                    nact++;
                }
                else
                {
                    pzero++;
                }
            }

            pzero /= datarr.Length;
            if( nact != 0.0 ) mn = sum / nact;

            if( nact == 1 )  /* Bogus Data array but do something reasonable */
            {
                alpha = 0.0;
                gamm = 1.0;
                beta = mn;
                return Convert.ToInt32( nact );
            }

            if( pzero == 1.0 ) /* They were all zeroes. */
            {
                alpha = 0.0;
                gamm = 1.0;
                beta = mn;
                return Convert.ToInt32( nact );
            }

            /* Use MLE */
            alpha = Math.Log( mn ) - sumlog / nact;
            gamm = (1.0 + Math.Sqrt( 1.0 + 4.0 * alpha / 3.0 )) / (4.0 * alpha);
            beta = mn / gamm;

            return Convert.ToInt32( nact );

        } 
        #endregion

        #region internal static double gamma_cdf( double beta, double gamm, double pzero, double x )
        /**************************************************************************
        *
        *  Compute probability of a<=x using incomplete gamma parameters.
        *
        *  Input:
        *      beta, gamma - gamma parameters
        *      pzero - probability of zero.
        *      x - value.
        *
        *  Return:
        *      Probability  a<=x.
        *
        ****************************************************************************/
        internal static double gamma_cdf( double beta, double gamm, double pzero, double x )
        {

            if( x <= 0.0 )
                return (pzero);
            else
                return (pzero + (1.0 - pzero) * gammap( gamm, x / beta ));

        } 
        #endregion


/*******************************************************************
 *
 *  Functions for the incomplete gamma functions P and Q
 *
 *                  1     /x  -t a-1
 *   P (a, x) = -------- |   e  t    dt,  a > 0
 *              Gamma(x)/ 0
 *
 *   Q (a, x) = 1 - P (a, x)
 *
 * Reference: Press, Flannery, Teukolsky, and Vetterling, 
 *        _Numerical Recipes_, pp. 160-163
 *
 * Thanks to kenny@cs.uiuc.edu
 *
 *********************************************************************/

        #region private static double gammser( double a, double x )
        /* Evaluate P(a,x) by its series representation.  Also put log gamma(a) into gln. */
        private static double gammser( double a, double x )
        {

            double ap, sum, del;
            int n;
            double gln = lgamma( a );

            if( x == 0.0 )
                return 0.0;

            ap = a;
            sum = 1.0 / a;
            del = sum;

            for( n = 0; n < _intMaxIterations; ++n )
            {
                sum += (del *= (x / ++ap));
                if( Math.Abs( del ) < _dblEpsilon * Math.Abs( sum ) )
                    break;
            }

            return sum * Math.Exp( -x + a * Math.Log( x ) - gln );

        } 
        #endregion

        #region private static double gammcf( double a, double x )
        /* Evaluate P(a,x) in its continued fraction representation.  Once again, return gln = log gamma (a). */
        private static double gammcf( double a, double x )
        {
            
            double gln = lgamma( a );
            double g = 0.0;
            double gold = 0.0;
            double a0 = 1.0;
            double a1 = x;
            double b0 = 0.0;
            double b1 = 1.0;
            double fac = 1.0;

            for( int n = 1; n <= _intMaxIterations; ++n )
            {
                double an = n;
                double ana = an - a;
                double anf;
                a0 = (a1 + a0 * ana) * fac;
                b0 = (b1 + b0 * ana) * fac;
                anf = an * fac;
                a1 = x * a0 + anf * a1;
                b1 = x * b0 + anf * b1;
                if( a1 != 0.0 )
                {
                    fac = 1.0 / a1;
                    g = b1 * fac;
                    if( Math.Abs( (g - gold) / g ) < _dblEpsilon )
                        break;
                    gold = g;
                }
            }

            return g * Math.Exp( -x + a * Math.Log( x ) - gln );

        }
        #endregion

        #region private static double gammap( double a, double x )
        /* Evaluate the incomplete gamma function P(a,x), choosing the most appropriate representation. */
        private static double gammap( double a, double x )
        {
            if( x < a + 1.0 )
                return gammser( a, x );
            else
                return 1.0 - gammcf( a, x );
        } 
        #endregion

        #region private static double lgamma( double gmm )
        private static double lgamma( double gmm )
        {

            double x, y, ser, tmp;

            double[] cof = new double[6];
            cof[0] = 76.18009173;
            cof[1] = -86.50532033;
            cof[2] = 24.01409822;
            cof[3] = -1.231739516;
            cof[4] = 0.120858003e-2;
            cof[5] = -0.536382e-5;

            x = y = gmm;
            tmp = x + 5.5;
            tmp = tmp - (x + 0.5) * Math.Log( tmp );
            ser = 1.00000000019005;

            for( int j = 0; j < cof.Length; j++ )
                ser = ser + cof[j] / ++y;

            return (-tmp + Math.Log( 2.50662827465 * ser / x ));

        } 
        #endregion




        #region these functions were in the original spi code, aren't used, but were kept for reference

        #region internal double[] ComputeSPIMonthlyOnly( int timeScale, double[] pp, int beginYear, int endYear, ref double[] beta, ref double[] gamm, ref double[] pzero ) -- not used
        ///// <summary>
        ///// Calculate indices assuming incomplete gamma distribution.
        ///// </summary>
        ///// <param name="timeScale">input - run length</param>
        ///// <param name="pp">input - prec array</param>
        ///// <param name="beginYear">input - begin year</param>
        ///// <param name="endYear">input - end year</param>
        ///// <param name="beta">output - beta param</param>
        ///// <param name="gamm">output - gamma param</param>
        ///// <param name="pzero">output - prob of x = 0</param>
        ///// <returns></returns>
        //internal double[] ComputeSPIMonthlyOnly( int timeScale, double[] pp, int beginYear, int endYear, ref double[] beta, ref double[] gamm, ref double[] pzero )
        //{

        //    int im;
        //    double alpha = 0;
        //    int intYearSpan = endYear - beginYear + 1;

        //    // Allocate temporary space 
        //    double[] SPI = new double[intYearSpan * 12];

        //    // The first nrun-1 index values will be missing.
        //    for( int i = 0; i < timeScale - 1; i++ )
        //        SPI[i] = SPI.Missing;

        //    // Sum nrun precip. values; 
        //    // store them in the appropriate index location.
        //    // If any value is missing; set the sum to missing.
        //    for( int j = timeScale - 1; j < intYearSpan * 12; j++ )
        //    {
        //        SPI[j] = 0.0;
        //        for( int i = 0; i < timeScale; i++ )
        //        {
        //            if( pp[j - i] != SPI.Missing )
        //                SPI[j] += pp[j - i];
        //            else
        //            {
        //                SPI[j] = SPI.Missing;
        //                break;
        //            }
        //        }
        //    }

        //    // For nrun<12, the monthly distributions will be substantially different, 
        //    // so we need to compute gamma parameters for each month starting with the (nrun-1)th. 

        //    List<double> tmp;

        //    for( int i = 0; i < 12; i++ )
        //    {
        //        tmp = new List<double>();

        //        for( int j = timeScale + i - 1; j < intYearSpan * 12; j += 12 )
        //        {
        //            if( SPI[j] != SPI.Missing )
        //                tmp.Add( SPI[j] );
        //        }

        //        // im is the calendar month; 0=jan...
        //        im = (timeScale + i - 1) % 12;

        //        // Here's where we do the fitting
        //        gamma_fit( tmp.ToArray(), ref alpha, ref beta[im], ref gamm[im], ref pzero[im] );
        //    }

        //    // Replace precip. sums stored in index with SPI's 
        //    for( int j = timeScale - 1; j < intYearSpan * 12; j++ )
        //    {

        //        if( SPI[j] != SPI.Missing )
        //        {
        //            im = j % 12;

        //            // Get the probability 
        //            SPI[j] = gamma_cdf( beta[im], gamm[im], pzero[im], SPI[j] );

        //            // Convert prob. to z value. 
        //            SPI[j] = inv_normal( SPI[j] );
        //        }
        //    }

        //    return SPI;

        //} 
        #endregion

        #region private double gamma_inv( double beta, double gamm, double pzero, double prob ) -- not used
        ///***************************************************************************
        //*
        //*  Compute inverse gamma function; i.e. return x given p where CDF(x) = p.
        //*
        //*  Input:
        //*      beta, gamma - gamma parameters
        //*      pzero - probability of zero.
        //*      prob - probability.
        //*
        //*  Return:
        //*      x as above.
        //*
        //*  Method:
        //*      We use a simple binary search to first bracket out initial
        //*      guess and then to refine our guesses until two guesses are within
        //*      tolerance (eps).  Is there a better way to do this?
        //*
        //***************************************************************************/
        //private double gamma_inv( double beta, double gamm, double pzero, double prob )
        //{
        //    int count = 0;
        //    double eps = 1.0e-7;
        //    double t_low, t_high, t, p_low, p_high, p;

        //    /* Check if prob < prob of zero */
        //    if( prob <= pzero )
        //        return (0.0);

        //    /* Otherwise adjust prob */
        //    prob = (prob - pzero) / (1.0 - pzero);

        //    /* Make initial guess */
        //    for( t_high = 2.0 * eps;
        //         (p_high = gamma_cdf( beta, gamm, pzero, t_high )) < prob;
        //         t_high *= 2.0 ) ;
        //    t_low = t_high / 2.0;
        //    p_low = gamma_cdf( beta, gamm, pzero, t_low );

        //    while( (t_high - t_low) > eps )
        //    {
        //        count++;
        //        t = (t_low + t_high) / 2.0;
        //        p = gamma_cdf( beta, gamm, pzero, t );

        //        if( p < prob )
        //        {
        //            t_low = t;
        //            p_low = p;
        //        }
        //        else
        //        {
        //            t_high = t;
        //            p_high = p;
        //        }
        //    }
        //    return ((t_low + t_high) / 2.0);
        //} 
        #endregion

        #region private double gammaq( double a, double x ) -- not used
        ///* Evaluate the incomplete gamma function Q(a,x), choosing the most appropriate representation. */
        //private double gammaq( double a, double x )
        //{
        //    if( x < a + 1.0 )
        //        return 1.0 - gammser( a, x );
        //    else
        //        return gammcf( a, x );
        //}
        #endregion

        #endregion


    }

}
