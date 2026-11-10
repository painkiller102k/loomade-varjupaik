// Väikesed abifunktsioonid

export const speciesList = [
  { name: 'Koer', emoji: '🐶' },
  { name: 'Kass', emoji: '🐱' },
  { name: 'Küülik', emoji: '🐰' },
  { name: 'Lind', emoji: '🦜' },
]

export function speciesEmoji(species) {
  const found = speciesList.find((s) => s.name.toLowerCase() === (species || '').toLowerCase())
  return found ? found.emoji : '🐾'
}

export const statusNames = {
  Available: 'Otsib kodu',
  Reserved: 'Broneeritud',
  Adopted: 'Leidis kodu',
}

export function ageText(age) {
  if (age === 0) return 'alla 1 aasta'
  if (age === 1) return '1 aasta'
  return age + ' aastat'
}
