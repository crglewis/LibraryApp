/** Renders a 0-5 star string, e.g. "★★★☆☆", rounding to the nearest whole star. */
export function getRatingStars(rating?: number): string {
  const rounded = Math.round(rating || 0);
  let stars = '';
  for (let i = 1; i <= 5; i++) stars += i <= rounded ? '★' : '☆';
  return stars;
}

/** Mean of `rating` across a list of reviews, or undefined when there are none. */
export function computeAverageRating(reviews: { rating: number }[]): number | undefined {
  return reviews.length ? reviews.reduce((sum, r) => sum + r.rating, 0) / reviews.length : undefined;
}
