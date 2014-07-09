﻿// Accord Statistics Library
// The Accord.NET Framework
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2013
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord.Statistics.Distributions.Univariate
{
    using System;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Math;


    /// <summary>
    ///   Mann-Whitney's U statistic distribution.
    /// </summary>
    /// 
    /// <remarks>
    ///   This is the distribution for the first sample statistic, U1. Some textbooks
    ///   (and statistical packages) use alternate definitions for U, which should be
    ///   compared with the appropriate statistic tables or alternate distributions.
    /// </remarks>
    /// 
    [Serializable]
    public class MannWhitneyDistribution : UnivariateContinuousDistribution
    {

        /// <summary>
        ///   Gets the number of observations in the first sample. 
        /// </summary>
        /// 
        public int Samples1 { get; private set; }

        /// <summary>
        ///   Gets the number of observations in the second sample.
        /// </summary>
        /// 
        public int Samples2 { get; private set; }

        /// <summary>
        ///   Gets the rank statistics for the distribution.
        /// </summary>
        /// 
        public double[] Ranks { get; private set; }


        private bool smallSample;
        private double[] table;

        /// <summary>
        ///   Constructs a Mann-Whitney's U-statistic distribution.
        /// </summary>
        /// 
        /// <param name="ranks">The rank statistics.</param>
        /// <param name="n1">The number of observations in the first sample.</param>
        /// <param name="n2">The number of observations in the second sample.</param>
        /// 
        public MannWhitneyDistribution(double[] ranks, int n1, int n2)
        {
            this.Ranks = ranks;
            this.Samples1 = n1;
            this.Samples2 = n2;
            int nt = n1 + n2;

            this.smallSample = (n1 <= 30 && n2 <= 30);

            if (smallSample)
            {
                // For a small sample (< 30) the distribution is exact.

                int nc = (int)Special.Binomial(nt, n1);
                table = new double[nc];

                int i = 0; // Consider all possible combinations of samples
                foreach (double[] combination in Combinatorics.Combinations(Ranks, n1))
                    table[i++] = USample1(combination, Samples2);

                Array.Sort(table);
            }
        }


        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   this distribution evaluated at point <c>k</c>.
        /// </summary>
        /// 
        /// <param name="u">A single point in the distribution range.</param>
        /// 
        /// <remarks>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.
        /// </remarks>
        /// 
        public override double DistributionFunction(double u)
        {
            if (!smallSample)
            {

                // Normal approximation
                double z = ((u + 0.5) - Mean) / Math.Sqrt(Variance);

                double p = NormalDistribution.Standard.DistributionFunction(Math.Abs(z));

                return p;
            }
            else
            {
                // For small samples (< 400) and if there are not very large
                // differences in samples sizes, this distribution is exact.

                for (int i = 0; i < table.Length; i++)
                    if (u <= table[i]) return i / (double)table.Length;

                return 1;
            }
        }


        /// <summary>
        ///   Gets the Mann-Whitney's U statistic for the smaller sample.
        /// </summary>
        /// 
        public static double UMinimum(double[] ranks, int n1, int n2)
        {
            // Split the rankings back and sum
            double[] rank1 = ranks.Submatrix(0, n1 - 1);
            double[] rank2 = ranks.Submatrix(n1, n1 + n2 - 1);

            double t1 = rank1.Sum();
            double t2 = rank2.Sum();

            double t1max = n1 * n2 + (n1 * (n1 + 1)) / 2.0;
            double t2max = n1 * n2 + (n2 * (n2 + 1)) / 2.0;

            double u1 = t1max - t1;
            double u2 = t2max - t2;

            return Math.Min(u1, u2);
        }

        /// <summary>
        ///   Gets the Mann-Whitney's U statistic for the first sample.
        /// </summary>
        /// 
        public static double USample1(double[] rank1, int n2)
        {
            int n1 = rank1.Length;
            double t1 = rank1.Sum();
            double t1max = n1 * n2 + (n1 * (n1 + 1)) / 2.0;
            double u1 = t1max - t1;
            return u1;
        }

        /// <summary>
        ///   Gets the Mann-Whitney's U statistic for the second sample.
        /// </summary>
        /// 
        public static double USample2(double[] rank2, int n1)
        {
            int n2 = rank2.Length;
            double t2 = rank2.Sum();
            double t2max = n2 * n1 + (n2 * (n2 + 1)) / 2.0;
            double u2 = t2max - t2;
            return u2;
        }

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public override object Clone()
        {
            return new MannWhitneyDistribution(Ranks, Samples1, Samples2);
        }


        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's mean value.</value>
        /// 
        public override double Mean
        {
            get
            {
                int n1 = Samples1;
                int n2 = Samples2;
                return (n1 * n2) / 2.0;
            }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's variance.</value>
        /// 
        public override double Variance
        {
            get
            {
                int n1 = Samples1;
                int n2 = Samples2;
                return (n1 * n2 * (n1 + n2 + 1)) / 12;
            }
        }

        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's entropy.</value>
        /// 
        public override double Entropy
        {
            get { throw new NotSupportedException(); }
        }


        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   this distribution evaluated at point <c>u</c>.
        /// </summary>
        /// 
        /// <param name="u">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The probability of <c>u</c> occurring
        ///   in the current distribution.
        /// </returns>
        /// 
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>u</c> will occur.
        /// </remarks>
        /// 
        public override double ProbabilityDensityFunction(double u)
        {
            // For all possible values for U, find how many
            // of them are equal to the requested value.

            int count = 0;
            for (int j = 0; j < table.Length; j++)
                if (table[j] == u) count++;
            return count / (double)table.Length;
        }

        /// <summary>
        ///   Gets the log-probability density function (pdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="u">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The logarithm of the probability of <c>u</c>
        ///   occurring in the current distribution.
        /// </returns>
        /// 
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>u</c> will occur.
        /// </remarks>
        /// 
        public override double LogProbabilityDensityFunction(double u)
        {
            int count = 0;
            for (int j = 0; j < table.Length; j++)
                if (table[j] == u) count++;
            return Math.Log(count) - Math.Log(table.Length);
        }
    }
}
