using Aranzadi.DocumentAnalysis.Messaging.Model.Response;
using Aranzadi.DocumentAnalysis.Messaging.Model.Response.Providers;

namespace Aranzadi.DocumentAnalysis.Models.Anaconda.Providers
{
    public class IssueProvider : IIssueProvider
    {

        private FragmentAnaconda fragmentAnaconda;

        public string Id { get => fragmentAnaconda.Id; }

		public string Title { get => fragmentAnaconda.Type; }

		public IssueProvider(FragmentAnaconda fragmentAnaconda)
        {
            ValidateFragment(fragmentAnaconda);
            this.fragmentAnaconda = fragmentAnaconda;
        }		

		public IEnumerable<Requirement> GetRequirements(Issue issue)
        {
			var li = new List<Requirement>();
			if (fragmentAnaconda.Events != null)
			{
				foreach (var theEvent in fragmentAnaconda.Events)
				{
					if (theEvent.Entities != null)
					{
						foreach (var theEntity in theEvent.Entities)
						{
							if (RequirementProvider.IsValidRequirement(theEntity))
							{
								li.Add(new Requirement(issue, new RequirementProvider(theEvent, theEntity)));
							}
						}
					}
				}
			}
			return li;
        }

        public IEnumerable<Term> GetTerms(Issue issue)
        {
            var li = new List<Term>();
            if (fragmentAnaconda.Events != null)
            {
                foreach (var theEvent in fragmentAnaconda.Events)
                {
                    if (theEvent.Entities != null)
                    {
                        foreach (var theEntity in theEvent.Entities)
                        {
                            if (TermProvider.IsValidTerm(theEntity))
                            {
                                li.Add(new Term(issue, new TermProvider(theEvent, theEntity)));
                            }
                        }
                    }
                }
            }
            return li;
        }

		private void ValidateFragment(FragmentAnaconda fragmentAnaconda)
		{
			if (fragmentAnaconda == null)
			{
				throw new ArgumentNullException(nameof(fragmentAnaconda));
			}
			if (string.IsNullOrWhiteSpace(fragmentAnaconda.Id))
			{
				throw new ArgumentException($"{nameof(fragmentAnaconda)}, {nameof(fragmentAnaconda.Id)} está vacío.");
			}
			if (string.IsNullOrWhiteSpace(fragmentAnaconda.Type))
			{
				throw new ArgumentException($"{nameof(fragmentAnaconda)}, {nameof(fragmentAnaconda.Type)} está vacío.");
			}
		}

	}
}
