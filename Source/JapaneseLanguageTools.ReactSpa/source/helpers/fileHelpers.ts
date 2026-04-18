export function getFileExtension(fileName: string) {
  // The implementation was taken from a Stack Overflow answer:
  // https://stackoverflow.com/a/12900504
  return fileName.slice(((fileName.lastIndexOf(".") - 1) >>> 0) + 2);
}
