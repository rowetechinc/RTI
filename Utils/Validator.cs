/*
 * Copyright 2011, Rowe Technology Inc. 
 * All rights reserved.
 * http://www.rowetechinc.com
 * https://github.com/rowetechinc
 * 
 * Redistribution and use in source and binary forms, with or without modification, are
 * permitted provided that the following conditions are met:
 * 
 *  1. Redistributions of source code must retain the above copyright notice, this list of
 *      conditions and the following disclaimer.
 *      
 *  2. Redistributions in binary form must reproduce the above copyright notice, this list
 *      of conditions and the following disclaimer in the documentation and/or other materials
 *      provided with the distribution.
 *      
 *  THIS SOFTWARE IS PROVIDED BY Rowe Technology Inc. ''AS IS'' AND ANY EXPRESS OR IMPLIED 
 *  WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
 *  FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> OR
 *  CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 *  CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 *  SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
 *  ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 *  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 *  ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *  
 * The views and conclusions contained in the software and documentation are those of the
 * authors and should not be interpreted as representing official policies, either expressed
 * or implied, of Rowe Technology Inc.
 * 
 * HISTORY
 * -----------------------------------------------------------------
 * Date            Initials    Version    Comments
 * -----------------------------------------------------------------
 * 10/17/2011      RC                     Initial coding
 * 02/01/2012      RC          1.14       Added string validate that also returns a bool.
 * 10/04/2012      RC          2.15       Fixed misspelling of ValidateNumericMinMax() and rounding numbers in error message.
 *                                         Added ValidatePositiveNumeric().
 * 10/07/2012      RC          2.15       Added ValidateMinMax().
 * 10/10/2012      RC          2.15       Added ValidateMin(int value, int min, out bool isGood).
 * 10/16/2012      RC          2.15       Fixed showing all the decimal places for the result in ValidateMinMax().
 * 
 */

using System;
namespace RTI
{
    /// <summary>
    /// Different validation methods.
    /// </summary>
    public class Validator
    {
        /// <summary>
        /// Constructor.
        /// Does nothing.  All methods static.
        /// </summary>
        public Validator()
        {

        }

        // Validate information
        #region Validators

        /// <summary>
        /// Validate a value that does not have a min or max.
        /// This will check if the value exist and can be
        /// converted to a number.
        /// </summary>
        /// <param name="value">Value to validate.</param>
        /// <param name="isGood">Set out if the value is good.</param>
        /// <returns>Empty String = Good value / String value of error </returns>
        public static string ValidateNumeric(string value, out bool isGood)
        {
            string result = "";
            isGood = true;

            if (string.IsNullOrEmpty(value))
            {
                result = "Value is empty";
                isGood = false;
            }

            double convertedValue = 0.0;
            // Try to convert the value to a double
            if (!Double.TryParse(value, out convertedValue))
            {
                isGood = false;
                return "Value must be numeric";
            }

            return result;
        }

        /// <summary>
        /// Validate a value that does not have a min or max.
        /// This will check if the value exist and can be
        /// converted to a number.  It will then check if it is positive.
        /// </summary>
        /// <param name="value">Value to validate.</param>
        /// <param name="isGood">Set out if the value is good.</param>
        /// <returns>Empty String = Good value / String value of error </returns>
        public static string ValidatePositiveNumeric(string value, out bool isGood)
        {
            string result = "";
            isGood = true;

            if (string.IsNullOrEmpty(value))
            {
                result = "Value is empty";
                isGood = false;
            }

            double convertedValue = 0.0;
            // Try to convert the value to a double
            if (!Double.TryParse(value, out convertedValue))
            {
                isGood = false;
                return "Value must be numeric";
            }

            // Check if the value is positive.
            if (convertedValue < 0)
            {
                isGood = false;
                return "Value must be positive.";
            }

            return result;
        }

        /// <summary>
        /// Validate a given value against a minimum and maximum value.
        /// </summary>
        /// <param name="value">Value to validate.</param>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        /// <param name="isGood">Set out if the value is good.</param>
        /// <returns>Empty String = Good value / String value of error </returns>
        public static string ValidateNumericMinMax(string value, double min, double max, out bool isGood)
        {
            string result = "";
            isGood = true;

            if (string.IsNullOrEmpty(value))
            {
                result = "Value is empty";
                isGood = false;
            }

            if (!ValidateMinMax(value, min, max))
            {
                result = String.Format("Value not within range of {0} to {1}", min.ToString("0.000"), max.ToString("0.000"));
                isGood = false;
            }

            return result;
        }

        /// <summary>
        /// Validate a given value against a minimum and maximum value.
        /// </summary>
        /// <param name="value">Value to validate.</param>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        /// <returns>Empty String = Good value / String value of error </returns>
        public static string ValidateNumericMinMax(string value, int min, int max)
        {
            string result = "";
            if (string.IsNullOrEmpty(value))
            {
                result = "Value is empty";
            }

            if (!ValidateMinMax(value, min, max))
            {
                result = String.Format("Value not within range of {0} to {1}", min.ToString("0"), max.ToString("0"));
            }

            return result;
        }

        /// <summary>
        /// Validate a string that it is not
        /// empty or null.
        /// </summary>
        /// <param name="value">String to validate.</param>
        /// <returns>Good = Empty string / Bad Value = Error message.</returns>
        public static string ValidateString(string value)
        {
            string result = "";
            if (string.IsNullOrEmpty(value))
            {
                result = "Value is empty";
            }

            return result;
        }

        /// <summary>
        /// Validate a string that it is not
        /// empty or null.
        /// </summary>
        /// <param name="value">String to validate.</param>
        /// <param name="isGood">Give output if the string was good.</param>
        /// <returns>Good = Empty string / Bad Value = Error message.</returns>
        public static string ValidateString(string value, out bool isGood)
        {
            isGood = true;
            string result = "";
            if (string.IsNullOrEmpty(value))
            {
                result = "Value is empty";
                isGood = false;
            }

            return result;
        }

        /// <summary>
        /// Validate a given value against a minimum value.
        /// </summary>
        /// <param name="value">Value to validate.</param>
        /// <param name="min">Minimum value.</param>
        /// <param name="isGood">Flag if the value was good.</param>
        /// <returns>Empty String = Good value / String value of error </returns>
        public static string ValidateMin(int value, int min, out bool isGood)
        {
            string result = "";
            isGood = true;

            if (!ValidateMin(value, min))
            {
                result = String.Format("Value is less then minimum value {0}", min.ToString("0"));
                isGood = false;
            }

            return result;
        }

        /// <summary>
        /// Validate a given value against a minimum value.
        /// </summary>
        /// <param name="value">Value to validate.</param>
        /// <param name="min">Minimum value.</param>
        /// <param name="isGood">Flag if the value was good.</param>
        /// <returns>Empty String = Good value / String value of error </returns>
        public static string ValidateMin(double value, double min, out bool isGood)
        {
            string result = "";
            isGood = true;

            if (!ValidateMin(value, min))
            {
                result = String.Format("Value is less then minimum value {0}", min.ToString("0.000"));
                isGood = false;
            }

            return result;
        }

        /// <summary>
        /// Validate a given value against a minimum and maximum value.
        /// </summary>
        /// <param name="value">Value to validate.</param>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        /// <param name="isGood">Flag if the value was good.</param>
        /// <returns>Empty String = Good value / String value of error </returns>
        public static string ValidateMinMax(double value, double min, double max, out bool isGood)
        {
            string result = "";
            isGood = true;

            if (!ValidateMinMax(value, min, max))
            {
                result = String.Format("Value not within range of {0} to {1}", min.ToString("0.000"), max.ToString("0.000"));
                isGood = false;
            }

            return result;
        }

        /// <summary>
        ///  Method to validate a value against a minimum and maximum value.
        /// </summary>
        /// <param name="value">Value to validate.</param>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        /// <returns>TRUE = Valid value / FALSE = Bad Value</returns>
        public static bool ValidateMinMax(string value, double min, double max)
        {
            double convertedValue = 0.0;

            // Try to convert the value to a double
            if (!Double.TryParse(value, out convertedValue))
            {
                return false;
            }


            // Check if value is within range
            if (convertedValue < min || convertedValue > max)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///  Method to validate a value against a minimum and maximum value.
        /// </summary>
        /// <param name="value">Value to validate.</param>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        /// <returns>TRUE = Valid value / FALSE = Bad Value</returns>
        public static bool ValidateMinMax(string value, int min, int max)
        {
            int convertedValue = 0;

            // Try to convert the value to a double
            if (!Int32.TryParse(value, out convertedValue))
            {
                return false;
            }

            // Check if value is within range
            if (convertedValue < min || convertedValue > max)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///  Method to validate a value against a minimum value.
        /// </summary>
        /// <param name="value">Value to validate.</param>
        /// <param name="min">Minimum value.</param>
        /// <returns>TRUE = Valid value / FALSE = Bad Value</returns>
        public static bool ValidateMin(double value, double min)
        {
            // Check if value is within range
            if (value < min)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///  Method to validate a value against a minimum and maximum value.
        /// </summary>
        /// <param name="value">Value to validate.</param>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        /// <returns>TRUE = Valid value / FALSE = Bad Value</returns>
        public static bool ValidateMinMax(double value, double min, double max)
        {
            // Check if value is within range
            if (value < min || value > max)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validate that none of the ranges are null.  If 
        /// any value is null then return false.
        /// </summary>
        /// <param name="dataset">Dataset to check.</param>
        /// <returns>TRUE =  All ranges given / FALSE = At least 1 ranges is null in dataset.</returns>
        public static bool ValidateEnsembleDataSet(DataSet.Ensemble dataset)
        {

            // Check if any of the ranges are null
            if (dataset == null ||
                dataset.EnsembleData == null ||
                dataset.AncillaryData == null ||
                //dataset.EnsembleData.DataSetID == null ||
                //dataset.EnsembleData.NumElements == null ||
                //dataset.EnsembleData.NumBeams == null ||
                //dataset.EnsembleData.DesiredPingCount == null ||
                //dataset.EnsembleData.ActualPingCount == null ||
                //dataset.EnsembleData.Status == null ||
                //dataset.EnsembleData.Year == null ||
                //dataset.EnsembleData.Month == null ||
                //dataset.EnsembleData.Day == null ||
                //dataset.EnsembleData.Hour == null ||
                //dataset.EnsembleData.Minute == null ||
                //dataset.EnsembleData.Second == null ||
                //dataset.EnsembleData.HSec == null ||
                //dataset.EnsembleData.EnsDateTime == null ||
                float.IsNaN(dataset.AncillaryData.FirstBinRange) ||
                float.IsNaN(dataset.AncillaryData.BinSize) ||
                 float.IsNaN(dataset.AncillaryData.FirstPingTime) ||
                 float.IsNaN(dataset.AncillaryData.LastPingTime) ||
                 float.IsNaN(dataset.AncillaryData.Heading) ||
                 float.IsNaN(dataset.AncillaryData.Pitch) ||
                 float.IsNaN(dataset.AncillaryData.Roll) ||
                 float.IsNaN(dataset.AncillaryData.WaterTemp) ||
                 float.IsNaN(dataset.AncillaryData.SystemTemp) ||
                 float.IsNaN(dataset.AncillaryData.Salinity) ||
                 float.IsNaN(dataset.AncillaryData.Pressure) ||
                 float.IsNaN(dataset.AncillaryData.TransducerDepth) ||
                 float.IsNaN(dataset.AncillaryData.SpeedOfSound)
                )
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}