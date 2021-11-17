using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class Condition
    {
        [SerializeField] private Disjunction[] and;
        
        public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
        {
            return and.All(disjunction => disjunction.Check(evaluators));
        }
        
        [Serializable]
        public class Disjunction
        {
            [SerializeField] private Predicate[] or;
            
            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                return or.Any(predicate => predicate.Check(evaluators));
            }
        }
        
        [Serializable]
        public class Predicate
        {
            [SerializeField] private EPredicate predicate;
            [SerializeField] private string[] parameters;
            [SerializeField] private bool negated;

            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                return evaluators.Select(evaluator => evaluator.Evaluate(predicate, parameters))
                    .Where(result => result != null)
                    .All(result => result != negated);
            }
        }
    }
}