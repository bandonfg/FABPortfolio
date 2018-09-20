export class Pagination {
    currentPage: number;
    itemPerPage: number;
    totalItems: number;
    totalPages: number;
}

export class PaginatedResult<T> {
    results: T;
    pagination: Pagination;
}
