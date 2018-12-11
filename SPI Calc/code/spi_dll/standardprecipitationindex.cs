using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;


[assembly: CLSCompliant( true )]
namespace NationalDroughtMitigationCenter
{


    public static class StandardPrecipitationIndex
    {

        public static readonly double[] Missing = { -99.0, -9999.0 };



        #region public static double[] Parse( double[] precipitation )
        public static double[] Parse( double[] precipitation )
        {

            double[] spi = new double[0];

            // check input data
            if( precipitation != null && precipitation.Length > 0 )
            {

                double alpha = 0;
                double beta = 0;
                double gamm = 0;
                double pzero = 0;

                // do the fitting using only non missing data
                GammaDistributionEstimatedProbabilities.gamma_fit( (from n in precipitation where !(from o in Missing select o).Contains(n) select n).ToArray(), ref alpha, ref beta, ref gamm, ref pzero );

                spi = new double[precipitation.Length];

                // replace precipitation sums with SPI's 
                for( int i = 0; i < precipitation.Length; i++ )
                {

                    double precipValue = Convert.ToDouble( precipitation[i].ToString( CultureInfo.InvariantCulture ), CultureInfo.InvariantCulture );
                    if( !Missing.Contains( precipValue ) )
                    {

                        // get probability 
                        spi[i] = GammaDistributionEstimatedProbabilities.gamma_cdf( beta, gamm, pzero, precipValue );

                        // convert probability to z value
                        spi[i] = GammaDistributionEstimatedProbabilities.inv_normal( spi[i] );

                    }
                    else
                    {
                        spi[i] = Missing[0];
                    }

                }

            }

            return spi;

        }
        #endregion

        #region public static DroughtPeriod[] DroughtPeriods( StandardPrecipitationIndexValue[] spi, double droughtLevel )
        public static DroughtPeriod[] DroughtPeriods( StandardPrecipitationIndexValue[] spi, double droughtLevel )
        {

            List<DroughtPeriod> lst = new List<DroughtPeriod>();

            if( droughtLevel > 0 )
                lst = DroughtPeriodsPositive( spi, droughtLevel );
            else
                lst = DroughtPeriodsNegative( spi, droughtLevel );

            return lst.ToArray();

        }
        #endregion

        #region public static StandardPrecipitationIndexFrequency[] Frequency( StandardPrecipitationIndexValue[] spi )
        public static StandardPrecipitationIndexFrequency[] Frequency( StandardPrecipitationIndexValue[] spi )
        {

            List<StandardPrecipitationIndexFrequency> lst = new List<StandardPrecipitationIndexFrequency>();

            var groups = from t in spi group t by Math.Round( t.SPI, 1 ) into g select new StandardPrecipitationIndexFrequency() { SPI = Math.Round( g.Key, 1 ), Count = g.Count(), Percentage = (double)g.Count() / (double)spi.Count() * 100.0 } ;
            groups = from t in groups orderby t.SPI select t;

            for( int i = 0; i < groups.Count(); i++ )
            {
                StandardPrecipitationIndexFrequency freq = new StandardPrecipitationIndexFrequency();
                freq.SPI = groups.ElementAt( i ).SPI;
                freq.Count = groups.ElementAt( i ).Count;
                freq.Percentage = groups.ElementAt( i ).Percentage;
                freq.RankingPercentile = (double)(i + 1) / (double)groups.Count() * 100.0;

                lst.Add( freq );
            }

            return lst.ToArray();

        }
        #endregion



        #region private static List<DroughtPeriod> DroughtPeriodsNegative( StandardPrecipitationIndexValue[] SPI, double droughtLevel )
        private static List<DroughtPeriod> DroughtPeriodsNegative( StandardPrecipitationIndexValue[] spi, double droughtLevel )
        {

            double intExit = 0;

            // let's make sure the data is ordered by Date.
            spi = (from n in spi orderby n.Date select n).ToArray();

            List<DroughtPeriod> lst = new List<DroughtPeriod>();
            bool inDroughtPeriod = false;
            DateTime? startDate = null;
            DateTime? endDate = null;
            int intDuration = 0;
            double dblSum = 0;
            double dblPeak = 999;


            // iterate though values
            foreach( StandardPrecipitationIndexValue val in spi )
            {

                if( !inDroughtPeriod )
                {

                    //start of drought period.
                    if( val.SPI <= droughtLevel && !Missing.Contains( val.SPI) )
                    {

                        inDroughtPeriod = true;
                        startDate = val.Date;
                        endDate = val.Date;

                        // create/add to stats
                        intDuration++;
                        dblSum += val.SPI;
                        if( val.SPI < dblPeak )
                            dblPeak = val.SPI;

                    }

                }
                else
                {

                    // end of drought period
                    if( val.SPI > intExit || Missing.Contains( val.SPI ) )
                    {

                        inDroughtPeriod = false;

                        // get median here.
                        double median;
                        List<StandardPrecipitationIndexValue> lstMedian = (from n in spi where n.Date >= startDate && n.Date <= endDate orderby n.SPI select n).ToList();
                        int intMedianCount = lstMedian.Count();
                        int midpoint = (intMedianCount - 1) / 2;
                        median = lstMedian.ElementAt( midpoint ).SPI;

                        if( intMedianCount % 2 == 0 )
                        {
                            median = (median + lstMedian.ElementAt( midpoint + 1 ).SPI) / 2;
                        }

                        DroughtPeriod prd = new DroughtPeriod();
                        prd.StartDate = startDate.Value;
                        prd.EndDate = val.Date;
                        prd.Duration = intDuration;
                        prd.Sum = dblSum;
                        prd.Peak = dblPeak;
                        prd.Average = prd.Sum / prd.Duration;
                        prd.Median = median;
                        lst.Add( prd );

                        // reset variables
                        intDuration = 0;
                        dblSum = 0;
                        dblPeak = 999;

                    }
                    else
                    {
                        // create/add to stats
                        intDuration++;
                        dblSum += val.SPI;
                        endDate = val.Date;
                        if( val.SPI < dblPeak )
                            dblPeak = val.SPI;

                    }
                }
            }

            return lst;

        }
        #endregion

        #region private static List<DroughtPeriod> DroughtPeriodsPositive( StandardPrecipitationIndexValue[] spi, double droughtLevel )
        private static List<DroughtPeriod> DroughtPeriodsPositive( StandardPrecipitationIndexValue[] spi, double droughtLevel )
        {

            double intExit = 0;

            // let's make sure the data is ordered by Date.
            spi = (from n in spi orderby n.Date select n).ToArray();

            List<DroughtPeriod> lst = new List<DroughtPeriod>();
            bool inDroughtPeriod = false;
            DateTime? startDate = null;
            DateTime? endDate = null;
            int intDuration = 0;
            double dblSum = 0;
            double dblPeak = -999;

            // iterate though values
            foreach( StandardPrecipitationIndexValue val in spi )
            {

                if( !inDroughtPeriod )
                {

                    //start of drought period.
                    if( val.SPI >= droughtLevel )
                    {

                        inDroughtPeriod = true;
                        startDate = val.Date;
                        endDate = val.Date;

                        // create/add to stats
                        intDuration++;
                        dblSum += val.SPI;
                        if( val.SPI > dblPeak )
                            dblPeak = val.SPI;

                    }

                }
                else
                {

                    // end of drought period
                    if( val.SPI < intExit || Missing.Contains( val.SPI ) )
                    {

                        inDroughtPeriod = false;

                        // median                        
                        List<StandardPrecipitationIndexValue> lstMedian = (from n in spi where n.Date >= startDate && n.Date <= endDate orderby n.SPI select n).ToList();
                        int intMedianCount = lstMedian.Count();
                        int midpoint = (intMedianCount - 1) / 2;
                        double median = lstMedian.ElementAt( midpoint ).SPI;

                        if( intMedianCount % 2 == 0 )
                            median = (median + lstMedian.ElementAt( midpoint + 1 ).SPI) / 2;

                        // saving record
                        DroughtPeriod prd = new DroughtPeriod();
                        prd.StartDate = startDate.Value;
                        prd.EndDate = val.Date;
                        prd.Duration = intDuration;
                        prd.Sum = dblSum;
                        prd.Peak = dblPeak;
                        prd.Average = prd.Sum / prd.Duration;
                        prd.Median = median;
                        lst.Add( prd );

                        // reset variables
                        intDuration = 0;
                        dblSum = 0;
                        dblPeak = -999;

                    }
                    else
                    {

                        endDate = val.Date;

                        // create/add to stats
                        intDuration++;
                        dblSum += val.SPI;
                        if( val.SPI > dblPeak )
                            dblPeak = val.SPI;

                    }
                }
            }

            return lst;

        }
        #endregion



    } // end class StandardPrecipitationIndex



    public class StandardPrecipitationIndexFrequency
    {
        public int Count;
        public double SPI;
        public double Percentage;
        public double RankingPercentile;
    }


    public struct StandardPrecipitationIndexValue
    {
        public DateTime Date;
        public double SPI;
    }


    public struct DroughtPeriod
    {
        public DateTime StartDate;
        public DateTime EndDate;
        public int Duration;
        public double Sum;
        public double Average;
        public double Median;
        public double Peak;
    }

}
