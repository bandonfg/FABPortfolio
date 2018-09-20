import { PortfolioPicture } from './portfolioPicture';
export interface Portfolios {
  id: number;
  project: string;
  description: string;
  projDuration: string;
  company: string;
  location: string;
  portfolioPictures?: PortfolioPicture;

}

// portfolioPictureId: number;
