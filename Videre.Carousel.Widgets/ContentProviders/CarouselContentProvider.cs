﻿using System.Collections.Generic;
using CodeEndeavors.Extensions;
using Videre.Core.ContentProviders;
using CoreServices = Videre.Core.Services;

namespace Videre.Carousel.Widgets.ContentProviders
{
    public class CarouselContentProvider : IWidgetContentProvider 
    {
        T IWidgetContentProvider.Get<T>(List<string> ids)
        {
            var id = ids.Count > 0 ? ids[0] : "";
            return Services.Carousel.GetById(id) as T; //hack:  [0]?   - exception when more than one?
        }

        public string GetJson(List<string> ids, string ignoreType = null)
        {
            var id = ids.Count > 0 ? ids[0] : "";
            return Services.Carousel.GetById(id).ToJson(ignoreType: ignoreType); //todo:  pass in ignoreType? //hack:  [0]? - exception when more than one?
        }

        public Dictionary<string, string> Import(string portalId, string ns, string json, Dictionary<string, string> idMap)
        {
            var ret = new Dictionary<string, string>();
            if (json != null)
            {
                var carousel = json.ToObject<Models.Carousel>();
                //menu.Name = Namespace;
                carousel.PortalId = portalId;
                ret[carousel.Id] = Services.Carousel.Import(portalId, carousel);
            }
            return ret;
        }

        public List<string> Save(string ns, string json)
        {
            var ret = new List<string>();
            if (json != null)
            {
                var carousel = json.ToObject<Models.Carousel>();
                ret.Add(Services.Carousel.Save(carousel));
            }
            return ret;
        }

        public void Delete(List<string> ids)
        {
            var id = ids.Count > 0 ? ids[0] : "";
            var Carousel = Services.Carousel.GetById(id);
            if (Carousel != null)
                Services.Carousel.Delete(Carousel.Id);
        }
    }

}
