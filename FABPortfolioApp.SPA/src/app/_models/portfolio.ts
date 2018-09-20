import { PortfolioFile } from './portfolioFile';

export interface Portfolio {
  id: number;
  project: string;
  description: string;
  url: string;
  from: string;
  to: string;
  company: string;
  location: string;
  portfolioFiles: PortfolioFile;
}

// portfolioPictureId: number;
// portfolioPictures: PortfolioPicture;
