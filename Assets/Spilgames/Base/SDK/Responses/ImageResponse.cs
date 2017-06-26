using UnityEngine;
using System.Collections;

namespace SpilGames.Unity.Base.SDK {
    /// <summary>
    /// ImageContext is used to store general context information about an  image being loaded.
    /// </summary>
    public class ImageContext {
        /// <summary>
        /// Can be: item, bundle or custom.
        /// </summary>
        public string imageType;

        /// <summary>
        /// Id of the item or bundle.
        /// </summary>
        public string id;

        /// <summary>
        /// Original web url as set in SLOT 
        /// </summary>
        public string imageUrl;
    }
}