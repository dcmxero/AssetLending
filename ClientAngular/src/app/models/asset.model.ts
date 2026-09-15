export interface AssetDto {
  id: number;
  name: string;
  description: string | null;
  serialNumber: string | null;
  status: string;
  isActive: boolean;
  assetCategoryId: number;
  assetCategoryName: string;
}

export interface CreateAssetDto {
  name: string;
  description: string | null;
  serialNumber: string | null;
  assetCategoryId: number;
}

export interface AssetCategoryDto {
  id: number;
  name: string;
  description: string | null;
}
