export interface LoanDto {
  id: number;
  assetId: number;
  assetName: string;
  borrowedById: number;
  borrowedByName: string;
  borrowedAt: string;
  dueDate: string;
  returnedAt: string | null;
  status: string;
}

export interface CreateLoanDto {
  assetId: number;
  borrowedById: number;
  dueDate: string;
}
