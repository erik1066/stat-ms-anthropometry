using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using System.Reflection;
using AnthStat.Statistics;

namespace Foundation.AnthStat.WebUI.Models
{
    /// <summary>
    /// A z-score result
    /// </summary>
    public sealed class ZScoreResult
    {
        /// <summary>
        /// The z-score
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        /// The percentile
        /// </summary>
        public double P { get; set; }

        /// <summary>
        /// The growth reference or standards used for calculating this result
        /// </summary>
        public string Reference { get; set; } = string.Empty;

        /// <summary>
        /// The indicator this result is for
        /// </summary>
        public Indicator Indicator { get; set; }

        /// <summary>
        /// The sex (male or female) this result is for
        /// </summary>
        public Sex Sex { get; set; }

        /// <summary>
        /// The first input measurement (often weight, height, length, circumference, or BMI)
        /// </summary>
        public double InputMeasurement1 { get; set; }

        /// <summary>
        /// The second input measurement (often age in months)
        /// </summary>
        public double InputMeasurement2 { get; set; }
    }
}