import BaseService from '../base.js';
import { RecordSearchRequestSchema, RecentSearchQuerySchema, PopularSearchQuerySchema, SuggestionsQuerySchema } from '../../schema/customer/search.js';

export default class SearchService extends BaseService {
  record(data) {
    const parsed = RecordSearchRequestSchema.parse(data);
    return this.post('/customer/search/record', parsed);
  }

  getRecent(data) {
    const parsed = RecentSearchQuerySchema.parse(data || {});
    return this.get('/customer/search/recent', parsed);
  }

  clearHistory() {
    return this.delete('/customer/search/history');
  }

  getPopular(data) {
    const parsed = PopularSearchQuerySchema.parse(data || {});
    return this.get('/customer/search/popular', parsed);
  }

  getSuggestions(data) {
    const parsed = SuggestionsQuerySchema.parse(data);
    return this.get('/customer/search/suggestions', parsed);
  }
}
