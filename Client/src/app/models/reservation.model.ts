export interface ReservationDto {
  id: number;
  assetId: number;
  assetName: string;
  reservedById: number;
  reservedByName: string;
  reservedAt: string;
  reservedUntil: string;
  isCancelled: boolean;
  isExpired: boolean;
}

export interface CreateReservationDto {
  assetId: number;
  reservedById: number;
  reservedUntil: string;
}
