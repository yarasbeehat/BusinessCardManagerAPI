using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using BusinessCardManager.Domain.Entities;

namespace BusinessCardManager.Domain.Dtos
{
    [XmlRoot("ArrayOfBusinessCardDto")]
    public class BusinessCardImport
    {
        [XmlElement("BusinessCard")]  
        public List<BusinessCard> BusinessCards { get; set; }
    }
}
