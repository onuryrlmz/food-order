import { client } from './service';

const fetcher = (url) => client.get(url).then((res) => res.data);
export default fetcher;
