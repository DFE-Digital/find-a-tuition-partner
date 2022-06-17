import { initAll } from 'govuk-frontend'
initAll()

import ResultsFilter from './javascript/results-filter'
var resultsFilter = new ResultsFilter()
resultsFilter.init()