import { PortfolioFile } from './portfolioFile';
export interface Portfolios {
  id: number;
  project: string;
  description: string;
  projDuration: string;
  company: string;
  location: string;
  portfolioFiles?: PortfolioFile;

}
