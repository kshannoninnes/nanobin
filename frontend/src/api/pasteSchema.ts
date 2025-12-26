import { z } from "zod";

export const CreatePasteResponseSchema = z.object({
    id: z.string().min(1),
    expires: z.iso.datetime( {offset: true} ),
});

export type CreatePasteResponse = z.infer<typeof CreatePasteResponseSchema>;

export const GetPasteResponseSchema = z.object({
    ciphertextBase64: z.string().min(1),
    ivBase64: z.string().min(1),
});

export type GetPasteResponse = z.infer<typeof GetPasteResponseSchema>;
