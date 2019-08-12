using UnityEngine;

namespace DUCK.Localisation
{
	public interface ILocalisedObject
	{
		LocalisedObject.LocalisedResourceType ResourceType { get; }
	}
}
