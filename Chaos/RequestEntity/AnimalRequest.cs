using Chaos.Api.Enums;
using Chaos.Api.Models;
using System;

namespace Chaos.Api.RequestEntity
{
    public class AnimalRequest
    {
        /// <summary>
        /// Request the type of the animal.
        /// </summary>
        public AnimalEnum TypeAnimal { get; set; }


    }
}