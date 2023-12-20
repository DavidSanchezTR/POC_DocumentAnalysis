using Aranzadi.DocumentAnalysis.Messaging.Model.Response;
using static Aranzadi.DocumentAnalysis.Messaging.Model.Response.ExplorationLevel;
using Aranzadi.DocumentAnalysis.Messaging.Model.Response.Providers;

namespace Aranzadi.DocumentAnalysis.Models.Anaconda.Providers
{
    public class JudicialNotificationProvider : IJudicialNotificationProvider
    {
        private DocumentAnalysisAnaconda documentAnalysisAnaconda;
        private const string COMPLAINANT = "DEMANDANTE";
        public JudicialNotificationProvider(DocumentAnalysisAnaconda? documentAnalysisAnaconda)
        {
            if (documentAnalysisAnaconda == null)
            {
                throw new ArgumentNullException(nameof(documentAnalysisAnaconda));
            }
            this.documentAnalysisAnaconda = documentAnalysisAnaconda;
        }

        public InformationNotification InformationNotification { get => new InformationNotification(new InformationProvider(documentAnalysisAnaconda)); }


        public IEnumerable<Issue> GetIssues(JudicialNotification judNoti)
        {
            var l = new List<Issue>();
            if (documentAnalysisAnaconda.Fragments != null)
            {
                foreach (var fragment in documentAnalysisAnaconda.Fragments)
                {
                    l.Add(new Issue(judNoti, new IssueProvider(fragment)));
                }
            }
            return l;
        }

        public ExplorationLevel Map { get => CalculateLevelsMap(documentAnalysisAnaconda); }


        public Process Process { get => new Process(new ProcessProvider(documentAnalysisAnaconda)); }



        private ExplorationLevel CalculateLevelsMap(DocumentAnalysisAnaconda documentAnalysisAnaconda)
        {
            ExplorationLevel levels = new ExplorationLevel();

            levels.Identificator = documentAnalysisAnaconda.Id;
            levels.Depth = 0;
            levels.Title = documentAnalysisAnaconda.Type;
            List<Property> props = new List<Property>();
            if (documentAnalysisAnaconda.Categories != null)
            {
                props.AddRange(GetCategories(documentAnalysisAnaconda.Categories));
            }
            if (documentAnalysisAnaconda.Entities != null)
            {
                props.AddRange(GetEntities(documentAnalysisAnaconda.Entities));
            }
            levels.Properties = props.ToList();

            levels.SubLevels = new List<ExplorationLevel>(GetFragments(documentAnalysisAnaconda.Fragments));


            return levels;
        }
        private List<ExplorationLevel> GetFragments(IEnumerable<FragmentAnaconda> fragments)
        {
            List<ExplorationLevel> subLevels = new List<ExplorationLevel>();
            if (fragments != null)
            {
                foreach (var frag in fragments)
                {
                    ExplorationLevel nivelFragment = new ExplorationLevel();
                    List<Property> props = new List<Property>();
                    nivelFragment.Depth = 1;
                    nivelFragment.Title = frag.Type;
                    nivelFragment.Container = frag.ContainerName;
                    nivelFragment.Identificator = frag.Id;
                    nivelFragment.IsMain = frag.IsMain;
                    if (frag.Categories != null)
                    {
                        props.AddRange(GetCategories(frag.Categories));
                    }
                    if (frag.Entities != null)
                    {
                        props.AddRange(GetEntities(frag.Entities));
                    }
                    nivelFragment.Properties = props.ToList();
                   
                    // subniveles de fragment -> events
                    nivelFragment.SubLevels = new List<ExplorationLevel>(GetEvents(frag.Events));
                    subLevels.Add(nivelFragment);
                }
            }
            return subLevels.OrderBy(x => !x.IsMain ? 1 : 0).ToList();
        }
        private List<ExplorationLevel> GetEvents(IEnumerable<EventAnaconda> events)
        {
            List<ExplorationLevel> subLevels = new List<ExplorationLevel>();
            if (events != null)
            {
                foreach (var eve in events)
                {
                    ExplorationLevel nivelEvent = new ExplorationLevel();
                    List<Property> props = new List<Property>();
                    nivelEvent.Depth = 2;
                    nivelEvent.Title = eve.Type;
                    nivelEvent.IsMain = eve.IsMain;
                    if (eve.Categories != null)
                    {
                        props.AddRange(GetCategories(eve.Categories));
                    }
                    if (eve.Entities != null)
                    {
                        props.AddRange(GetEntities(eve.Entities));
                    }
                    nivelEvent.Properties = props.ToList();
                    subLevels.Add(nivelEvent);
                }
            }
            return subLevels;
        }
        private List<Property> GetCategories(IEnumerable<CategoryAnaconda> categories)
        {
            List<Property> props = new List<Property>();
            foreach (var cat in categories)
            {
                Property prop = new Property()
                {
                    PropertyType = Property.eType.Categorized,
                    Key = cat.Type,
                    Value = cat.Label
                };
                props.Add(prop);
            }
            return props;
        }
        private List<Property> GetEntities(IEnumerable<EntityAnaconda> entities)
        {
            List<Property> props = new List<Property>();
            foreach (var ent in entities)
            {
                Property prop = new Property()
                {
                    PropertyType = Property.eType.Free,
                    Key = ent.Type,
                    Value = ent.Value
                };
                if (!string.IsNullOrEmpty(ent.Type) && ent.Type.ToUpper() == COMPLAINANT && ent.Children != null && ent.Children.Any())
                {
                    prop.Value = GetChildrenStringToComplainant(ent.Children);
                }
                props.Add(prop);
            }
            return props;
        }

        private string GetChildrenStringToComplainant(IEnumerable<EntityAnaconda> children)
        {
            List<string> demandantValueList = new List<string>();
            foreach (var child in children)
            {
                demandantValueList.Add(child.Value);
            }
            return string.Join("; ", demandantValueList);
        }
    }
}
