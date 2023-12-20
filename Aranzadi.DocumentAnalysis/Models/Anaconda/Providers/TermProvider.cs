using Aranzadi.DocumentAnalysis.Messaging.Model.Response;
using System.Text.RegularExpressions;
using Aranzadi.DocumentAnalysis.Messaging.Model.Response.Providers;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace Aranzadi.DocumentAnalysis.Models.Anaconda.Providers
{
	public class TermProvider : ITermProvider
	{

		internal const string TERM_PLAZO = "Plazo";

		private EventAnaconda eventAnaconda;
		private EntityAnaconda entity;

		public int? Days { get; private set; }

		public string PeriodDescription { get; private set; }

		public string Text { get; private set; }

		public TermProvider(EventAnaconda eventAnaconda, EntityAnaconda entity)
		{
			ValidateParameters(eventAnaconda, entity);
			this.eventAnaconda = eventAnaconda;
			this.entity = entity;
			CalculateDaysAndDescription();
			CalculateText();
		}

		private static void ValidateParameters(EventAnaconda eventAnaconda, EntityAnaconda entity)
		{
			if (eventAnaconda == null)
			{
				throw new ArgumentNullException(nameof(eventAnaconda));
			}

			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			if (!IsValidTerm(entity))
			{
				throw new ArgumentException("La entity no es un Term: " + nameof(entity));
			}
		}

		internal static bool IsValidTerm(EntityAnaconda entityAnaconda)
		{
			if (entityAnaconda == null)
			{
				return false;
			}

			return TERM_PLAZO.Equals(entityAnaconda.Type);
		}

		private void CalculateText()
		{
			if (string.IsNullOrWhiteSpace(eventAnaconda.Type))
			{
				this.Text = string.Empty;
			}
			else
			{
				this.Text = eventAnaconda.Type;
			}
		}

		private void CalculateDaysAndDescription()
		{
			if (string.IsNullOrWhiteSpace(entity.Value))
			{
				this.Days = null;
				this.PeriodDescription = string.Empty;
			}
			else
			{
				this.Days = 0;
				this.PeriodDescription = entity.Value.Trim();
				Regex reg = new Regex(@"^\s*(\d+)\s*(d[ií]as?)\s*$", RegexOptions.IgnoreCase);
				var m = reg.Matches(entity.Value);
				if (m != null & m.Count > 0)
				{
					int v = 0;
					if (int.TryParse(m[0].Groups[1].ToString(), out v))
					{
						this.Days = v;
						this.PeriodDescription = m[0].Groups[2].ToString();
					}
				}
			}
		}
	}
}
