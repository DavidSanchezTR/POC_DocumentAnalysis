using Aranzadi.DocumentAnalysis.Models.Anaconda.Providers.Attributes;
using Aranzadi.DocumentAnalysis.Messaging.Model.Response.Providers;

namespace Aranzadi.DocumentAnalysis.Models.Anaconda.Providers
{
    public class PersonsProvider : IPersonsProvider
    {
		internal const string ENTITY_ACTOR = "Actor";
		internal const string ENTITY_DEMANDADO = "Demandado";
		internal const string ENTITY_NAME = "Nombre";
		internal const string ENTITY_DEMANDANTE = "Demandante";

		internal const string ENTITY_DNI = "Dni-Pasaporte";
		internal const string ENTITY_DNI2 = "Número de identificación";

		internal const string ENTITY_LAWYER = "Abogado";
		internal const string ENTITY_ATTORNEY = "Procurador";
		
		private EntityAnaconda actorEntity;

        public PersonsProvider(EntityAnaconda actorEntity)
        {
            if (actorEntity == null)
            {
                throw new ArgumentNullException(nameof(actorEntity));
            }
            InitializeProperties();
            if (actorEntity.Children != null)
            {
                EntityAttribute.SetPropertiesByAtribute<ChildEntityAttribute>(actorEntity.Children, this);
            }
            else
            {
				EntityAttribute.SetPropertiesByAtribute<ChildEntityAttribute>(new List<EntityAnaconda>() { actorEntity }, this);
			}
        }
        private void InitializeProperties()
        {
            this.Name = string.Empty;
            this.DNI = string.Empty;
            this.Lawyer = string.Empty;
            this.Attorney = string.Empty;
        }

        [ChildEntity(ENTITY_NAME)]
		[ChildEntity(ENTITY_ACTOR)]
		[ChildEntity(ENTITY_DEMANDADO)]
		[ChildEntity(ENTITY_DEMANDANTE)]		
		public string Name { get; private set; }

		[ChildEntity(ENTITY_DNI)]
		[ChildEntity(ENTITY_DNI2)]
		public string DNI { get; private set; }

        [ChildEntity(ENTITY_LAWYER)]
        public string Lawyer { get; private set; }

        [ChildEntity(ENTITY_ATTORNEY)]
        public string Attorney { get; private set; }
    }
}
