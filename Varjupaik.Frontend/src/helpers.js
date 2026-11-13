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

export const applicationStatusNames = {
  New: 'Ootel',
  Approved: 'Kinnitatud',
  Rejected: 'Tagasi lükatud',
}

export function dateText(value) {
  return new Date(value).toLocaleString('et-EE', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  })
}

export function dateOnly(value) {
  return new Date(value).toLocaleDateString('et-EE', { day: '2-digit', month: '2-digit', year: 'numeric' })
}
