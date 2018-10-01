#pragma warning disable 1591 // disables the warnings about missing Xml code comments

using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Foundation.AnthStat.WebUI.Models;
using Swashbuckle.AspNetCore.Annotations;
using AnthStat.Statistics;

namespace Foundation.AnthStat.WebUI.Controllers
{
    [Route("api/1.0/who2007")]
    [ApiController]
    public sealed class StatisticsWho2007Controller : ControllerBase
    {
        private static WHO2007 _whoReference = new WHO2007();

        public StatisticsWho2007Controller()
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
                    Reference = "WHO2007",
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