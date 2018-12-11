using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StandardPrecipitationIndex
{


    #region public struct SPIRawRequest
    public struct SPIRawRequest
    {
        public InputOptions options;
        public List<InputData> data;
    } 
    #endregion

    #region public struct InputOptions
    public struct InputOptions
    {
        public int BeginYear;
        public int EndYear;
        public int TimeScale;
    } 
    #endregion

    #region public struct InputData
    public struct InputData
    {
        //        yyyy ww prec
        //Where:
        //yyyy : years; values > ENDYR and <BEGYR are skipped.
        //ww : 1-52 week numbers.
        //prec : precipitation
        //Special codes:
        //-99.00 = Missing
        public int year;
        public int month;
        public long precipitation;
    } 
    #endregion




    public class SPIMonthlyOnly
    {

        public const double _MISSING = -99.0;
        public const int _NLEN = 12;


        #region public double[] Parse( SPIRawRequest req )
        public double[] Parse( SPIRawRequest req )
        {

            double[] prec;  // Array of raw data
            double[] beta = new double[12];
            double[] gamm = new double[12];
            double[] pzero = new double[12];
            double[] spi;
            int intYear;

            int intYearSpan = req.options.EndYear - req.options.BeginYear + 1;

            // initializing prec array to MISSING
            prec = new double[intYearSpan * 12];
            for( int i = 0; i < intYearSpan * 12; i++ )
                prec[i] = _MISSING;

            for( int q = 0; q < req.data.Count; q++ )
            {

                intYear = req.data[q].year - req.options.BeginYear;

                // save the value if it's in the requested date range
                if( intYear >= 0 && intYear <= intYearSpan - 1 )
                    prec[intYear * 12 + req.data[q].month - 1] = req.data[q].precipitation / 100.0;

            }

            // Compute SPI's
            GammaDistributionEstimatedProbabilities spiGamma = new GammaDistributionEstimatedProbabilities();
            spi = spiGamma.ComputeSPIMonthlyOnly( req.options.TimeScale, prec, req.options.BeginYear, req.options.EndYear, ref beta, ref gamm, ref pzero );

            return spi;

        } 
        #endregion


    } // end public class SPI

}
