using Aranzadi.DocumentAnalysis.Messaging.Model.Response;
using Aranzadi.DocumentAnalysis.Models.Anaconda.Providers.Attributes;
using Aranzadi.DocumentAnalysis.Messaging.Model.Response.Providers;

namespace Aranzadi.DocumentAnalysis.Models.Anaconda.Providers
{
    public class ProcessProvider : IProcessProvider
    {

		internal const string ENTITY_AUTO = "Numero de procedimiento";
		internal const string ENTITY_AUTO2 = "Número de procedimiento";
		internal const string ENTITY_ORGANO = "Órgano";
		internal const string ENTITY_ORGANO_JUDICIAL = "Órgano judicial";
		internal const string ENTITY_PROCEDURE = "Procedimiento";
		internal const string ENTITY_NIG = "NIG";
		internal const string ENTITY_MATERIA = "Materia";
		internal const string ENTITY_ACTIVEPART_ACTOR = "Actor";
		internal const string ENTITY_ACTIVEPART_DEMANDANTE = "Demandante";
		internal const string ENTITY_PASSIVEPART_DEMANDADO = "Demandado";

		internal const string CATEGORY_PROCEDURE_TYPE = "Tipo de procedimiento";
		internal const string CATEGORY_MATERIA = "Materia";

		[DocumentAnalysisEntity(ENTITY_ORGANO)]
		[DocumentAnalysisEntity(ENTITY_ORGANO_JUDICIAL)]		
		public string Court { get; private set; }

		[DocumentAnalysisEntity(ENTITY_NIG)]
		public string NIG { get; private set; }

		[DocumentAnalysisEntity(ENTITY_PROCEDURE)]
		[DocumentAnalysisCategory(CATEGORY_PROCEDURE_TYPE)]
		public string Procedure { get; private set; }

		[DocumentAnalysisEntity(ENTITY_MATERIA)]
		[DocumentAnalysisCategory(CATEGORY_MATERIA)]
		public string Topic { get; private set; }

		[DocumentAnalysisEntity(ENTITY_AUTO)]
		[DocumentAnalysisEntity(ENTITY_AUTO2)]
		public string Judgement { get; private set; }

		private DocumentAnalysisAnaconda documentAnalysisAnaconda;

        public ProcessProvider(DocumentAnalysisAnaconda documentAnalysisAnaconda)
        {
            if (documentAnalysisAnaconda == null)
            {
                throw new ArgumentNullException(nameof(documentAnalysisAnaconda));
            }
            this.documentAnalysisAnaconda = documentAnalysisAnaconda;
            InitializeProperties();
            if (documentAnalysisAnaconda.Entities != null)
            {
                EntityAttribute.SetPropertiesByAtribute<DocumentAnalysisEntityAttribute>(documentAnalysisAnaconda.Entities, this);
            }
			if (documentAnalysisAnaconda.Categories != null)
			{
				CategoryAttribute.SetPropertiesByAtribute<DocumentAnalysisCategoryAttribute>(documentAnalysisAnaconda.Categories, this);
			}
		}

        private void InitializeProperties()
        {
            this.Court = string.Empty;
            this.Procedure = string.Empty;
            this.Judgement = string.Empty;
            this.NIG = string.Empty;
            this.Topic = string.Empty;
        }  

		public IEnumerable<PersonsInvolved> GetActivePart(Process process)
		{
			var l = new List<PersonsInvolved>();
			if (documentAnalysisAnaconda.Entities != null)
			{
				foreach (var actorEntity in documentAnalysisAnaconda.Entities.Where(x => new List<string>() { ENTITY_ACTIVEPART_ACTOR, ENTITY_ACTIVEPART_DEMANDANTE }.Contains(x.Type)))
				{
					PersonsProvider personProvider = new PersonsProvider(actorEntity);

					var involved = new PersonsInvolved(personProvider);

					l.Add(involved);
				};
			}

			return l;
		}

		public IEnumerable<PersonsInvolved> GetPassivePart(Process process)
		{
			var l = new List<PersonsInvolved>();
			if (documentAnalysisAnaconda.Entities != null)
			{
				foreach (var demandadoEntity in documentAnalysisAnaconda.Entities.Where(x => ENTITY_PASSIVEPART_DEMANDADO.Equals(x.Type)))
				{
					PersonsProvider personProvider = new PersonsProvider(demandadoEntity);

					var involved = new PersonsInvolved(personProvider);

					l.Add(involved);
				};
			}

			return l;
		}

	}
}
