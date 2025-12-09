using StepTune.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace StepTune.Domain.Music.ValueObjects
{
    public sealed class SongFile:IEquatable<SongFile>
    {
        public string FileUrl { get; }
        public string FileType { get; }
        public long FileSize { get; }

        private static readonly string[] AllowedTypes = { ".mp3", ".wav" };
        private const long MaxFileSize = 20_000_000; // 20MB

        private SongFile(string fileUrl, string fileType, long fileSize)
        {
            FileUrl = fileUrl;
            FileType = fileType;
            FileSize = fileSize;
        }
        public static Result<SongFile> Create(string fileUrl, string fileType, long fileSize)
        {
            if (!AllowedTypes.Contains(fileType.ToLower()))
                return Result.Failure<SongFile>("Only .mp3 and .wav files are allowed ");
            if (fileSize < 0)
                return Result.Failure<SongFile>("File size must be a positive number");
            if (fileSize > MaxFileSize)
                return Result.Failure<SongFile>("File size must be lower than 20 MB.");
            if (string.IsNullOrWhiteSpace(fileUrl))
                return Result.Failure<SongFile>("Invalid file Url");

            var songFile = new SongFile(fileUrl, fileType, fileSize);

            return Result.Success<SongFile>(songFile);

        }
        public bool Equals(SongFile? other)
        {
            if(other == null) return false;
            return FileUrl == other.FileUrl && FileType == other.FileType && FileSize == other.FileSize;
        }

        public override bool Equals(object? obj)
        {
            return obj is SongFile other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FileUrl, FileType, FileSize);
        }
    }
}
