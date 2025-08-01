using UnityEngine;

namespace SylhShrinkerCartPlus.Components
{
    /// <summary>
    /// Permet de sauvegarder une fois pour toutes le scale d'origine de l'objet,
    /// pour éviter les erreurs cumulatives dues aux interpolations successives.
    /// </summary>
    public class OriginalScaleTracker : MonoBehaviour
    {
        public Vector3 InitialScale { get; private set; }

        private bool initialized = false;

        public void InitializeIfNeeded()
        {
            if (!initialized)
            {
                InitialScale = transform.localScale;
                initialized = true;
            }
        }

        private void Awake()
        {
            InitializeIfNeeded();
        }
    }
}