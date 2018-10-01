#pragma warning disable 1591 // disables the warnings about missing Xml code comments

using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Foundation.AnthStat.WebUI.Models;
using Swashbuckle.AspNetCore.Annotations;
using AnthStat.Statistics;

namespace Foundation.AnthStat.WebUI.Controllers
{
    [Route("api/1.0/who2006")]
    [ApiController]
    public sealed class StatisticsWho2006Controller : ControllerBase
    {
        private static WHO2006 _whoReference = new WHO2006();

        public StatisticsWho2006Controller()
        {
        }

        /// <summary>
        /// Gets a z-score for the BMI-for-age indicator
        /// </summary>
        /// <param name="sex">The sex of the person being measured, 0 for male and 1 for female</param>
        /// <param name="bodyMassIndex">The BMI of the person being measured</param>
        /// <param name="ageInMonths">The age of the person being measured in months</param>
        /// <returns>Z-score and accompanying percentile</returns>
        [Produces("application/json")]
        [HttpGet("zscore/bmi-for-age")]
        [SwaggerResponse(200, "If the z-score was calculated successfully", typeof(ZScoreResult))]
        [SwaggerResponse(400, "If one or more inputs were invalid")]
        public IActionResult BmiForAge([FromQuery] Sex sex, [FromQuery] double bodyMassIndex, [FromQuery] double ageInMonths) =>  Calculate(sex, Indicator.BodyMassIndexForAge, bodyMassIndex, ageInMonths);

        /// <summary>
        /// Gets a z-score for the Head circumference-for-age indicator
        /// </summary>
        /// <param name="sex">The sex of the person being measured, 0 for male and 1 for female</param>
        /// <param name="headCircumference">The head circumference of the person being measured (centimeters)</param>
        /// <param name="ageInMonths">The age of the person being measured in months</param>
        /// <returns>Z-score and accompanying percentile</returns>
        [Produces("application/json")]
        [HttpGet("zscore/hc-for-age")]
        [SwaggerResponse(200, "If the z-score was calculated successfully", typeof(ZScoreResult))]
        [SwaggerResponse(400, "If one or more inputs were invalid")]
        public IActionResult HcForAge([FromQuery] Sex sex, [FromQuery] double headCircumference, [FromQuery] double ageInMonths) =>  Calculate(sex, Indicator.HeadCircumferenceForAge, headCircumference, ageInMonths);

        /// <summary>
        /// Gets a z-score for the Length-for-age indicator
        /// </summary>
        /// <param name="sex">The sex of the person being measured, 0 for male and 1 for female</param>
        /// <param name="length">The length of the person being measured (centimeters)</param>
        /// <param name="ageInMonths">The age of the person being measured in months</param>
        /// <returns>Z-score and accompanying percentile</returns>
        [Produces("application/json")]
        [HttpGet("zscore/length-for-age")]
        [SwaggerResponse(200, "If the z-score was calculated successfully", typeof(ZScoreResult))]
        [SwaggerResponse(400, "If one or more inputs were invalid")]
        public IActionResult LengthForAge([FromQuery] Sex sex, [FromQuery] double length, [FromQuery] double ageInMonths) =>  Calculate(sex, Indicator.LengthForAge, length, ageInMonths);

        /// <summary>
        /// Gets a z-score for the Height-for-age indicator
        /// </summary>
        /// <param name="sex">The sex of the person being measured, 0 for male and 1 for female</param>
        /// <param name="height">The height of the person being measured (centimeters)</param>
        /// <param name="ageInMonths">The age of the person being measured in months</param>
        /// <returns>Z-score and accompanying percentile</returns>
        [Produces("application/json")]
        [HttpGet("zscore/height-for-age")]
        [SwaggerResponse(200, "If the z-score was calculated successfully", typeof(ZScoreResult))]
        [SwaggerResponse(400, "If one or more inputs were invalid")]
        public IActionResult HeightForAge([FromQuery] Sex sex, [FromQuery] double height, [FromQuery] double ageInMonths) =>  Calculate(sex, Indicator.HeightForAge, height, ageInMonths);

        /// <summary>
        /// Gets a z-score for the Weight-for-age indicator
        /// </summary>
        /// <param name="sex">The sex of the person being measured, 0 for male and 1 for female</param>
        /// <param name="weight">The weight of the person being measured (kilograms)</param>
        /// <param name="ageInMonths">The age of the person being measured in months</param>
        /// <returns>Z-score and accompanying percentile</returns>
        [Produces("application/json")]
        [HttpGet("zscore/weight-for-age")]
        [SwaggerResponse(200, "If the z-score was calculated successfully", typeof(ZScoreResult))]
        [SwaggerResponse(400, "If one or more inputs were invalid")]
        public IActionResult WeightForAge([FromQuery] Sex sex, [FromQuery] double weight, [FromQuery] double ageInMonths) =>  Calculate(sex, Indicator.WeightForAge, weight, ageInMonths);

        /// <summary>
        /// Gets a z-score for the Weight-for-height indicator
        /// </summary>
        /// <param name="sex">The sex of the person being measured, 0 for male and 1 for female</param>
        /// <param name="weight">The weight of the person being measured (kilograms)</param>
        /// <param name="height">The height of the person being measured (centimeters)</param>
        /// <returns>Z-score and accompanying percentile</returns>
        [Produces("application/json")]
        [HttpGet("zscore/weight-for-height")]
        [SwaggerResponse(200, "If the z-score was calculated successfully", typeof(ZScoreResult))]
        [SwaggerResponse(400, "If one or more inputs were invalid")]
        public IActionResult WeightForHeight([FromQuery] Sex sex, [FromQuery] double weight, [FromQuery] double height) =>  Calculate(sex, Indicator.WeightForHeight, weight, height);

        /// <summary>
        /// Gets a z-score for the Weight-for-length indicator
        /// </summary>
        /// <param name="sex">The sex of the person being measured, 0 for male and 1 for female</param>
        /// <param name="weight">The weight of the person being measured (kilograms)</param>
        /// <param name="length">The length of the person being measured (centimeters)</param>
        /// <returns>Z-score and accompanying percentile</returns>
        [Produces("application/json")]
        [HttpGet("zscore/weight-for-length")]
        [SwaggerResponse(200, "If the z-score was calculated successfully", typeof(ZScoreResult))]
        [SwaggerResponse(400, "If one or more inputs were invalid")]
        public IActionResult WeightForLength([FromQuery] Sex sex, [FromQuery] double weight, [FromQuery] double length) =>  Calculate(sex, Indicator.WeightForLength, weight, length);

        /// <summary>
        /// Gets a z-score for the Arm circumference-for-age indicator
        /// </summary>
        /// <param name="sex">The sex of the person being measured, 0 for male and 1 for female</param>
        /// <param name="armCircumference">The arm circumference of the person being measured (centimeters)</param>
        /// <param name="ageInMonths">The age of the person being measured in months</param>
        /// <returns>Z-score and accompanying percentile</returns>
        [Produces("application/json")]
        [HttpGet("zscore/ac-for-age")]
        [SwaggerResponse(200, "If the z-score was calculated successfully", typeof(ZScoreResult))]
        [SwaggerResponse(400, "If one or more inputs were invalid")]
        public IActionResult AcForAge([FromQuery] Sex sex, [FromQuery] double armCircumference, [FromQuery] double ageInMonths) =>  Calculate(sex, Indicator.ArmCircumferenceForAge, armCircumference, ageInMonths);

        /// <summary>
        /// Gets a z-score for the Subscapular skinfold-for-age indicator
        /// </summary>
        /// <param name="sex">The sex of the person being measured, 0 for male and 1 for female</param>
        /// <param name="subscapularSkinfold">The subscapular skinfold of the person being measured (millimeters)</param>
        /// <param name="ageInMonths">The age of the person being measured in months</param>
        /// <returns>Z-score and accompanying percentile</returns>
        [Produces("application/json")]
        [HttpGet("zscore/ssf-for-age")]
        [SwaggerResponse(200, "If the z-score was calculated successfully", typeof(ZScoreResult))]
        [SwaggerResponse(400, "If one or more inputs were invalid")]
        public IActionResult SsfForAge([FromQuery] Sex sex, [FromQuery] double subscapularSkinfold, [FromQuery] double ageInMonths) =>  Calculate(sex, Indicator.SubscapularSkinfoldForAge, subscapularSkinfold, ageInMonths);

        /// <summary>
        /// Gets a z-score for the Triceps skinfold-for-age indicator
        /// </summary>
        /// <param name="sex">The sex of the person being measured, 0 for male and 1 for female</param>
        /// <param name="tricepsSkinfold">The triceps skinfold of the person being measured (millimeters)</param>
        /// <param name="ageInMonths">The age of the person being measured in months</param>
        /// <returns>Z-score and accompanying percentile</returns>
        [Produces("application/json")]
        [HttpGet("zscore/tsf-for-age")]
        [SwaggerResponse(200, "If the z-score was calculated successfully", typeof(ZScoreResult))]
        [SwaggerResponse(400, "If one or more inputs were invalid")]
        public IActionResult TsfForAge([FromQuery] Sex sex, [FromQuery] double tricepsSkinfold, [FromQuery] double ageInMonths) =>  Calculate(sex, Indicator.TricepsSkinfoldForAge, tricepsSkinfold, ageInMonths);

        private IActionResult Calculate(Sex sex, Indicator indicator, double measurement1, double measurement2)
        {
            double z = default(double);
            bool success = _whoReference.TryCalculateZScore(Indicator.BodyMassIndexForAge, measurement1, measurement2, sex, ref z);
            if (success)
            {
                var scoreResult = new ZScoreResult()
                {
                    Z = z,
                    P = StatisticsHelper.CalculatePercentile(z),
                    Reference = "WHO2006",
                    Sex = sex,
                    Indicator = indicator,
                    InputMeasurement1 = measurement1,
                    InputMeasurement2 = measurement2
                };
                return Ok(scoreResult);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}

#pragma warning restore 1591